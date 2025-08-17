using FSI.Authentication.Application.DTOs.Auth;
using FSI.Authentication.Application.Interfaces.Repositories;
using FSI.Authentication.Application.Interfaces.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSI.Authentication.Application.UseCases.RegisterUser
{
    public sealed class RegisterUserHandler
    {
        private readonly IUserAccountRepository userRepo;
        private readonly IPasswordHasher hasher;
        private readonly AuthDomainService domainService;
        private readonly IEventPublisher publisher;

        public RegisterUserHandler(
            IUserAccountRepository userRepo,
            IPasswordHasher hasher,
            AuthDomainService domainService,
            IEventPublisher publisher)
        {
            this.userRepo = userRepo; this.hasher = hasher;
            this.domainService = domainService; this.publisher = publisher;
        }

        public async Task<RegisterUserResponse> HandleAsync(RegisterUserCommand cmd, CancellationToken ct)
        {
            var email = Email.Create(cmd.Email);
            var name = PersonName.Create(cmd.FirstName, cmd.LastName);

            var strength = domainService.EnsurePasswordStrength(cmd.Password);
            if (!strength.IsSuccess) throw new ValidationAppException(strength.Error!);

            var existing = await userRepo.GetByEmailAsync(email, ct);
            if (existing is not null) throw new ConflictException($"E-mail já cadastrado: {email}");

            var profileName = FSI.Authentication.Domain.ValueObjects.ProfileName.Create(cmd.Profile);
            var profile = new Profile(profileName);

            var hash = PasswordHash.FromHashed(hasher.Hash(cmd.Password));
            var user = UserAccount.Register(email, name, hash, profile);

            await userRepo.AddAsync(user, ct);
            await publisher.PublishAsync(
                new UserRegisteredNotification(user.Id, user.Email.Value, user.Name.ToString(), profileName.Value, DateTime.UtcNow), ct);

            return new RegisterUserResponse(user.Id, user.Email.Value, user.Name.ToString(), profileName.Value);
        }
    }
}

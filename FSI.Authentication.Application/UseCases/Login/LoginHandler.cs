using FSI.Authentication.Application.DTOs.Auth;
using FSI.Authentication.Application.Interfaces.Repositories;
using FSI.Authentication.Application.Interfaces.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSI.Authentication.Application.UseCases.Login
{
    public sealed class LoginHandler
    {
        private readonly IUserAccountRepository repo;
        private readonly IPasswordHasher hasher;
        private readonly ITokenProvider tokenProvider;
        private readonly AuthDomainService domainService;
        private readonly IEventPublisher publisher;

        public LoginHandler(
            IUserAccountRepository repo,
            IPasswordHasher hasher,
            ITokenProvider tokenProvider,
            AuthDomainService domainService,
            IEventPublisher publisher)
        {
            this.repo = repo; this.hasher = hasher; this.tokenProvider = tokenProvider;
            this.domainService = domainService; this.publisher = publisher;
        }

        public async Task<LoginResponse> HandleAsync(LoginCommand cmd, CancellationToken ct)
        {
            var email = Email.Create(cmd.Email);
            var user = await repo.GetByEmailAsync(email, ct);
            if (user is null) throw new UnauthorizedException("Credenciais inválidas");

            if (!domainService.CanSignIn(user))
                throw new UnauthorizedException("Usuário bloqueado ou inativo");

            if (!hasher.Verify(cmd.Password, user.PasswordHash.Value))
            {
                user.RegisterFailedLogin(new SystemClock(), 5, TimeSpan.FromMinutes(15));
                await repo.UpdateAsync(user, ct);
                await publisher.PublishAsync(new FailedLoginNotification(user.Id, user.Email.Value, DateTime.UtcNow), ct);
                throw new UnauthorizedException("Credenciais inválidas");
            }

            user.ResetFailures();
            await repo.UpdateAsync(user, ct);

            var profile = user.Profile.Name.Value;
            var permissions = user.Profile.Permissions.Select(p => p.Code).ToArray();
            var (access, exp) = tokenProvider.CreateAccessToken(user.Id, user.Email.Value, profile, permissions);
            var refresh = tokenProvider.CreateRefreshToken(user.Id);

            return new LoginResponse(access, refresh, exp, profile, permissions);
        }
    }

    file-static class SystemClock : FSI.Authentication.Domain.Abstractions.IClock
    {
        public DateTime UtcNow => DateTime.UtcNow;
    }
}

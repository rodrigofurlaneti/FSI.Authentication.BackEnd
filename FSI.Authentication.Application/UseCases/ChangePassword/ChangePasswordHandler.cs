using FSI.Authentication.Application.Interfaces.Repositories;
using FSI.Authentication.Application.Interfaces.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSI.Authentication.Application.UseCases.ChangePassword
{
    public sealed class ChangePasswordHandler
    {
        private readonly IUserAccountRepository repo;
        private readonly IPasswordHasher hasher;
        private readonly FSI.Authentication.Domain.Services.AuthDomainService domainService;
        private readonly FSI.Authentication.Application.Interfaces.Services.IEventPublisher publisher;

        public ChangePasswordHandler(
            IUserAccountRepository repo,
            IPasswordHasher hasher,
            FSI.Authentication.Domain.Services.AuthDomainService domainService,
            FSI.Authentication.Application.Interfaces.Services.IEventPublisher publisher)
        {
            this.repo = repo; this.hasher = hasher; this.domainService = domainService; this.publisher = publisher;
        }

        public async Task HandleAsync(ChangePasswordCommand cmd, CancellationToken ct)
        {
            var email = Email.Create(cmd.Email);
            var user = await repo.GetByEmailAsync(email, ct) ?? throw new NotFoundException("Usuário não encontrado");
            if (!hasher.Verify(cmd.CurrentPassword, user.PasswordHash.Value))
                throw new UnauthorizedException("Senha atual incorreta");

            var strong = domainService.EnsurePasswordStrength(cmd.NewPassword);
            if (!strong.IsSuccess) throw new ValidationAppException(strong.Error!);

            var newHash = PasswordHash.FromHashed(hasher.Hash(cmd.NewPassword));
            user.ChangePassword(newHash);
            await repo.UpdateAsync(user, ct);
            await publisher.PublishAsync(new PasswordChangedNotification(user.Id, DateTime.UtcNow), ct);
        }
    }
}

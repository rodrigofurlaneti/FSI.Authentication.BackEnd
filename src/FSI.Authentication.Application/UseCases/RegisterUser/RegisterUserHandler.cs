using System;
using System.Threading;
using System.Threading.Tasks;
using FSI.Authentication.Application.Interfaces.Repositories;
using FSI.Authentication.Application.Interfaces.Services;
using FSI.Authentication.Domain.Abstractions;
using FSI.Authentication.Domain.Aggregates;
using FSI.Authentication.Domain.Services;

namespace FSI.Authentication.Application.UseCases.RegisterUser
{
    public sealed class RegisterUserHandler
    {
        private readonly IUserAccountRepository _users;
        private readonly IPasswordHasher _hasher;
        private readonly IAuthDomainService _domain;
        private readonly IClock _clock;

        public RegisterUserHandler(
            IUserAccountRepository users,
            IPasswordHasher hasher,
            IAuthDomainService domain,
            IClock clock)
        {
            _users = users;
            _hasher = hasher;
            _domain = domain;
            _clock = clock;
        }

        public async Task<Guid> Handle(RegisterUserCommand cmd, CancellationToken ct)
        {
            var emailVo = new FSI.Authentication.Domain.ValueObjects.Email(cmd.Email);
            var existing = await _users.GetByEmailAsync(emailVo, ct);
            if (existing is not null)
                throw new InvalidOperationException("E-mail já cadastrado.");

            var hash = _hasher.Hash(cmd.Password);

            var user = new UserAccount(
                Guid.NewGuid(),
                emailVo,              // se o Domain aceitar string, você pode trocar para cmd.Email
                cmd.FirstName,
                cmd.LastName,
                hash,
                cmd.ProfileName,
                isActive: true
            );

            if (!_domain.CanSignIn(user, _clock, out var reason))
                throw new InvalidOperationException(reason ?? "Usuário inválido para autenticação.");

            await _users.AddAsync(user, ct);
            return user.UserId;
        }
    }
}

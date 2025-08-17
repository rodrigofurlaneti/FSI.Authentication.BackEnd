using System;
using System.Threading;
using System.Threading.Tasks;
using FSI.Authentication.Application.Interfaces.Repositories;
using FSI.Authentication.Application.Interfaces.Services;
using FSI.Authentication.Domain.Abstractions;
using FSI.Authentication.Domain.Services;
using AppRepos = FSI.Authentication.Application.Interfaces.Repositories;
using AppServ = FSI.Authentication.Application.Interfaces.Services;

namespace FSI.Authentication.Application.UseCases.Login
{
    public sealed record LoginCommand(string Email, string Password);

    public sealed class LoginResult
    {
        public bool IsSuccessful { get; init; }
        public string? Error { get; init; }
        public string? Token { get; init; }
        public DateTimeOffset? ExpiresAtUtc { get; init; }
        public Guid? UserId { get; init; }
        public string? Email { get; init; }
        public string? ProfileName { get; init; }

        public static LoginResult Fail(string error) => new() { IsSuccessful = false, Error = error };

        public static LoginResult Success(string token, DateTimeOffset expires, Guid userId, string email, string profile)
            => new()
            {
                IsSuccessful = true,
                Token = token,
                ExpiresAtUtc = expires,
                UserId = userId,
                Email = email,
                ProfileName = profile
            };
    }

    public sealed class LoginHandler
    {
        private readonly AppRepos.IUserAccountRepository _users;
        private readonly AppServ.IPasswordHasher _hasher;
        private readonly AppServ.ITokenProvider _tokens;
        private readonly IAuthDomainService _domain;
        private readonly IClock _clock;

        public LoginHandler(AppRepos.IUserAccountRepository users, AppServ.IPasswordHasher hasher, AppServ.ITokenProvider tokens, IAuthDomainService domain, IClock clock)
        {
            _users = users;
            _hasher = hasher;
            _tokens = tokens;
            _domain = domain;
            _clock = clock;
        }

        public async Task<LoginResult> Handle(LoginCommand command, CancellationToken ct)
        {
            var emailVo = new FSI.Authentication.Domain.ValueObjects.Email(command.Email);
            var user = await _users.GetByEmailAsync(emailVo, ct);
            if (user is null)
                return LoginResult.Fail("Credenciais inválidas.");

            if (!_hasher.Verify(command.Password, user.PasswordHash))
            {
                user.RegisterFailedLogin(_clock);
                await _users.UpdateAsync(user, ct);
                return LoginResult.Fail("Credenciais inválidas.");
            }

            if (!_domain.CanSignIn(user, _clock, out var reason))
                return LoginResult.Fail(reason ?? "Não autorizado.");

            user.ResetFailedLogins(_clock);
            await _users.UpdateAsync(user, ct);

            var token = _tokens.CreateToken(user);
            return LoginResult.Success(token.Token, token.ExpiresAtUtc, user.UserId, user.Email, user.ProfileName);
        }
    }
}

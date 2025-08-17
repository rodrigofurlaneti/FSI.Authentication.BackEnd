using System;
using System.Threading;
using System.Threading.Tasks;
using FSI.Authentication.Application.UseCases.Login;
using FSI.Authentication.Application.UseCases.RegisterUser;

namespace FSI.Authentication.Application.Services
{
    public sealed class AuthenticationAppService
    {
        private readonly LoginHandler _login;
        private readonly RegisterUserHandler _register;

        public AuthenticationAppService(LoginHandler login, RegisterUserHandler register)
        {
            _login = login;
            _register = register;
        }

        public Task<LoginResult> LoginAsync(LoginCommand command, CancellationToken ct)
            => _login.Handle(command, ct);

        public Task<Guid> RegisterAsync(RegisterUserCommand command, CancellationToken ct)
            => _register.Handle(command, ct);
    }
}

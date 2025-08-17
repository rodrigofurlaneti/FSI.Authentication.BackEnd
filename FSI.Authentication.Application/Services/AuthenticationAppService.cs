using System;
using System.Threading;
using System.Threading.Tasks;
using FSI.Authentication.Application.UseCases.Login;
using FSI.Authentication.Application.UseCases.RegisterUser;
using FSI.Authentication.Application.UseCases.ChangePassword;

namespace FSI.Authentication.Application.Services
{
    public sealed class AuthenticationAppService
    {
        private readonly LoginHandler _login;
        private readonly RegisterUserHandler _register;
        private readonly ChangePasswordHandler _changePassword;

        public AuthenticationAppService(
            LoginHandler login,
            RegisterUserHandler register,
            ChangePasswordHandler changePassword)
        {
            _login = login;
            _register = register;
            _changePassword = changePassword;
        }

        public Task<LoginResult> LoginAsync(LoginCommand command, CancellationToken ct)
            => _login.Handle(command, ct);

        public Task<Guid> RegisterAsync(RegisterUserCommand command, CancellationToken ct)
            => _register.Handle(command, ct);

        public Task ChangePasswordAsync(string emailOrId, string currentPassword, string newPassword, CancellationToken ct)
        {
            // Se seu ChangePasswordCommand exigir outro formato, ajuste aqui.
            var cmd = new ChangePasswordCommand(emailOrId, currentPassword, newPassword, newPassword);
            return _changePassword.Handle(cmd, ct);
        }
    }
}

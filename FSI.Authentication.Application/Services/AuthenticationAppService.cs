using FSI.Authentication.Application.DTOs.Auth;
using FSI.Authentication.Application.DTOs.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSI.Authentication.Application.Services
{
    public sealed class AuthenticationAppService
    {
        private readonly UseCases.Login.LoginHandler login;
        private readonly UseCases.RegisterUser.RegisterUserHandler register;
        private readonly UseCases.GetProfile.GetProfileHandler profile;
        private readonly UseCases.ChangePassword.ChangePasswordHandler changePwd;
        private readonly UseCases.ChangeProfile.ChangeProfileHandler changeProfile;

        public AuthenticationAppService(
            UseCases.Login.LoginHandler login,
            UseCases.RegisterUser.RegisterUserHandler register,
            UseCases.GetProfile.GetProfileHandler profile,
            UseCases.ChangePassword.ChangePasswordHandler changePwd,
            UseCases.ChangeProfile.ChangeProfileHandler changeProfile)
        {
            this.login = login; this.register = register; this.profile = profile;
            this.changePwd = changePwd; this.changeProfile = changeProfile;
        }

        public Task<LoginResponse> LoginAsync(LoginRequest r, CancellationToken ct)
            => login.HandleAsync(new UseCases.Login.LoginCommand(r.Email, r.Password), ct);

        public Task<RegisterUserResponse> RegisterAsync(RegisterUserRequest r, CancellationToken ct)
            => register.HandleAsync(new UseCases.RegisterUser.RegisterUserCommand(
                r.Email, r.FirstName, r.LastName, r.Password, r.Profile), ct);

        public Task<UserProfileDto> GetProfileAsync(string email, CancellationToken ct)
            => profile.HandleAsync(new UseCases.GetProfile.GetProfileQuery(email), ct);

        public Task ChangePasswordAsync(string email, string currentPassword, string newPassword, CancellationToken ct)
            => changePwd.HandleAsync(new UseCases.ChangePassword.ChangePasswordCommand(
                email, currentPassword, newPassword), ct);

        public Task<ChangeProfileResponse> ChangeProfileAsync(string email, string newProfile, CancellationToken ct)
            => changeProfile.HandleAsync(new UseCases.ChangeProfile.ChangeProfileCommand(email, newProfile), ct);
    }
}

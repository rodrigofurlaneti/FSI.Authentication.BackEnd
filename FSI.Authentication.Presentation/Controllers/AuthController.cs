using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using FSI.Authentication.Application.Services;
using FSI.Authentication.Application.DTOs.Auth;
using FSI.Authentication.Presentation.Models.Auth;

namespace FSI.Authentication.Presentation.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public sealed class AuthController : ControllerBase
    {
        private readonly AuthenticationAppService _app;

        public AuthController(AuthenticationAppService app) => _app = app;

        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<ActionResult<RegisterUserResponse>> Register(RegisterUserRequestModel model, CancellationToken ct)
        {
            var result = await _app.RegisterAsync(
                new RegisterUserRequest(model.Email, model.FirstName, model.LastName, model.Password, model.Profile), ct);

            return CreatedAtAction(nameof(GetProfile), new { email = result.Email }, result);
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public Task<LoginResponse> Login(LoginRequestModel model, CancellationToken ct)
            => _app.LoginAsync(new LoginRequest(model.Email, model.Password), ct);

        [HttpGet("profile")]
        [Authorize]
        public Task<FSI.Authentication.Application.DTOs.Shared.UserProfileDto> GetProfile([FromQuery] string? email, CancellationToken ct)
            => _app.GetProfileAsync(email ?? User.FindFirst("email")?.Value ?? User.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress")?.Value ?? throw new InvalidOperationException("E-mail não encontrado no token."), ct);

        [HttpPost("change-password")]
        [Authorize]
        public async Task<IActionResult> ChangePassword(ChangePasswordRequestModel model, CancellationToken ct)
        {
            var email = model.Email ?? User.FindFirst("email")?.Value
                ?? User.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress")?.Value
                ?? throw new InvalidOperationException("E-mail não encontrado no token.");

            await _app.ChangePasswordAsync(email, model.CurrentPassword, model.NewPassword, ct);
            return NoContent();
        }

        [HttpPost("change-profile")]
        [Authorize(Policy = "Users.Read")] // exemplo de policy
        public async Task<ActionResult<ChangeProfileResponse>> ChangeProfile(ChangeProfileRequestModel model, CancellationToken ct)
        {
            var email = model.Email ?? User.FindFirst("email")?.Value
                ?? User.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress")?.Value
                ?? throw new InvalidOperationException("E-mail não encontrado no token.");

            var resp = await _app.ChangeProfileAsync(email, model.NewProfile, ct);
            return Ok(resp);
        }
    }
}

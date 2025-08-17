using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using FSI.Authentication.Application.Services;
using FSI.Authentication.Presentation.Models.Auth;
using FSI.Authentication.Application.UseCases.Login;
using FSI.Authentication.Application.UseCases.RegisterUser;
using System;
using System.Threading;
using System.Threading.Tasks;

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
        public async Task<ActionResult<Guid>> Register(RegisterUserRequestModel model, CancellationToken ct)
        {
            var id = await _app.RegisterAsync(new RegisterUserCommand(model.Email, model.FirstName, model.LastName, model.Password, model.Profile),
                ct);

            // Sem rota de profile, devolva 201 com Location vazio ou 200 OK:
            // return Ok(new { id });
            return Created(string.Empty, id);
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public Task<LoginResult> Login(LoginRequestModel model, CancellationToken ct)
            => _app.LoginAsync(new LoginCommand(model.Email, model.Password), ct);

        [HttpPost("change-password")]
        [Authorize]
        public async Task<IActionResult> ChangePassword(ChangePasswordRequestModel model, CancellationToken ct)
        {
            var email = model.Email
                ?? User.FindFirst("email")?.Value
                ?? User.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress")?.Value
                ?? throw new InvalidOperationException("E-mail não encontrado no token.");

            await _app.ChangePasswordAsync(email, model.CurrentPassword, model.NewPassword, ct);
            return NoContent();
        }
    }
}

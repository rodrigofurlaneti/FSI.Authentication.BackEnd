using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Reflection;

namespace FSI.Authentication.Presentation.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public sealed class AdminController : ControllerBase
    {
        [HttpGet("health")]
        [AllowAnonymous]
        public IActionResult Health() => Ok(new { status = "ok", serverTimeUtc = DateTime.UtcNow });

        [HttpGet("info")]
        [Authorize(Policy = "Users.Read")]
        public IActionResult Info()
        {
            var asm = Assembly.GetExecutingAssembly();
            return Ok(new { name = asm.GetName().Name, version = asm.GetName().Version?.ToString() });
        }
    }
}

using FSI.Authentication.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Diagnostics;
using System.Reflection;
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
    public sealed class AdminController : ControllerBase
    {
        private readonly AdminAppService _adminAppService;

        public AdminController(AdminAppService adminAppService)
        {
            _adminAppService = adminAppService ?? throw new ArgumentNullException(nameof(adminAppService));
        }

        [HttpGet("health")]
        [AllowAnonymous]
        public IActionResult Health() => Ok(new { status = "ok", serverTimeUtc = DateTime.UtcNow });

        [HttpGet("health/database")]
        [AllowAnonymous]
        public async Task<IActionResult> HealthDatabase(CancellationToken ct)
        {
            var sw = Stopwatch.StartNew();
            try
            {
                var isOk = await _adminAppService.TestDatabaseAsync(ct); // retorna bool (true = OK)

                sw.Stop();
                if (isOk)
                {
                    return Ok(new
                    {
                        status = "ok",
                        serverTimeUtc = DateTime.UtcNow,
                        elapsedMs = sw.ElapsedMilliseconds
                    });
                }

                return StatusCode(503, new
                {
                    status = "degraded",
                    error = "db_unavailable",
                    serverTimeUtc = DateTime.UtcNow,
                    elapsedMs = sw.ElapsedMilliseconds
                });
            }
            catch (Exception ex)
            {
                sw.Stop();
                return StatusCode(503, new
                {
                    status = "degraded",
                    error = "db_unavailable",
                    message = ex.GetBaseException().Message,
                    serverTimeUtc = DateTime.UtcNow,
                    elapsedMs = sw.ElapsedMilliseconds
                });
            }
        }

        [HttpGet("info")]
        [Authorize(Policy = "Users.Read")]
        public IActionResult Info()
        {
            var asm = Assembly.GetExecutingAssembly();
            return Ok(new { name = asm.GetName().Name, version = asm.GetName().Version?.ToString() });
        }
    }
}

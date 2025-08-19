using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using FSI.Authentication.Application.DTOs.Geo;
using FSI.Authentication.Application.Services;
using System.Threading;
using System.Threading.Tasks;

namespace FSI.Authentication.Presentation.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class GeolocationController : ControllerBase
{
    private readonly GeoLoggingAppService _app;

    public GeolocationController(GeoLoggingAppService app) => _app = app;

    [HttpPost]
    [AllowAnonymous]
    [ProducesResponseType(204)]
    public async Task<IActionResult> Post([FromBody] ClientContextPayload payload, CancellationToken ct)
    {
        if (payload is null) return BadRequest("Empty payload.");
        await _app.LogAsync(payload, ct);
        return NoContent();
    }
}

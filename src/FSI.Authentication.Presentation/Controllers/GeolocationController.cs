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
    private readonly GeoLoggingAppService _geoLoggingAppService;

    public GeolocationController(GeoLoggingAppService geoLoggingAppService)
    {
        _geoLoggingAppService = geoLoggingAppService;
    } 

    [HttpPost]
    [AllowAnonymous]
    [Consumes("application/json")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status415UnsupportedMediaType)]
    public async Task<IActionResult> Post([FromBody] ClientContextPayload payload, CancellationToken ct)
    {
        if (payload is null) return BadRequest("Empty payload.");

        var remoteIp = HttpContext.Connection.RemoteIpAddress?.ToString();
        var xff = Request.Headers["X-Forwarded-For"].FirstOrDefault();
        var ua = Request.Headers["User-Agent"].ToString();

        await _geoLoggingAppService.LogAsync(payload, ct);

        return NoContent();
    }

    // Opcional: facilita depuração de CORS/preflight
    [HttpOptions]
    [AllowAnonymous]
    public IActionResult Options() => Ok();
}

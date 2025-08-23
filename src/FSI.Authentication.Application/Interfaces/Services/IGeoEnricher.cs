using FSI.Authentication.Application.DTOs.Geo;

namespace FSI.Authentication.Application.Interfaces.Services
{
    public interface IGeoEnricher
    {
        Task<GeoEnrichmentDto> EnrichAsync(ClientContextPayload payload, CancellationToken ct);
    }
}

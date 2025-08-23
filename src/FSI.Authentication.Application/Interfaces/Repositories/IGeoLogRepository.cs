using System.Threading;
using System.Threading.Tasks;
using FSI.Authentication.Application.DTOs.Geo;

namespace FSI.Authentication.Application.Interfaces.Repositories;

public interface IGeoLogRepository
{
    Task<long> InsertAsync(ClientContextPayload payload, CancellationToken ct);
    Task InsertEnrichmentAsync(long geoClientLogId, GeoEnrichmentDto geoEnrichmentDto, CancellationToken ct);
}
using System.Threading;
using System.Threading.Tasks;
using FSI.Authentication.Application.DTOs.Geo;
using FSI.Authentication.Application.Interfaces.Repositories;

namespace FSI.Authentication.Application.Services;

public sealed class GeoLoggingAppService
{
    private readonly IGeoLogRepository _repo;
    public GeoLoggingAppService(IGeoLogRepository repo) => _repo = repo;

    public Task LogAsync(ClientContextPayload payload, CancellationToken ct)
        => _repo.InsertAsync(payload, ct);
}
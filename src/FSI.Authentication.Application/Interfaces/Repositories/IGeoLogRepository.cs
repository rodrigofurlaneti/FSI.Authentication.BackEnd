using System.Threading;
using System.Threading.Tasks;
using FSI.Authentication.Application.DTOs.Geo;

namespace FSI.Authentication.Application.Interfaces.Repositories;

public interface IGeoLogRepository
{
    Task InsertAsync(ClientContextPayload payload, CancellationToken ct);
}
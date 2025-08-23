using FSI.Authentication.Application.DTOs.Geo;
using FSI.Authentication.Application.Interfaces;
using FSI.Authentication.Application.Interfaces.Repositories;
using FSI.Authentication.Application.Interfaces.Services;
using System.Threading;
using System.Threading.Tasks;

namespace FSI.Authentication.Application.Services;

public sealed class GeoLoggingAppService
{
    private readonly IGeoLogRepository _geoLogRepository;
    private readonly IGeoEnricher _geoEnricher;
    private readonly IUnitOfWork _unitOfWork;

    public GeoLoggingAppService(IGeoLogRepository geoLogRepository, IGeoEnricher geoEnricher, IUnitOfWork unitOfWork)
    {
        _geoLogRepository = geoLogRepository;
        _geoEnricher = geoEnricher;
        _unitOfWork = unitOfWork;
    } 

    public async Task<long> LogAsync(ClientContextPayload payload, CancellationToken ct)
    {
        long id;

        // Fase A: transação curta para o "bruto", Latitude, Longitude ...
        await using (var tx = await _unitOfWork.BeginAsync(ct))
        {
            id = await _geoLogRepository.InsertAsync(payload, ct);
            await tx.CommitAsync();
        }

        // Fase B: chamadas externas (sem transação), pegar os dados Nome da rua, municipio, cep ...
        var enriched = await _geoEnricher.EnrichAsync(payload, ct);

        // Fase C: Insert do enriquecimento os detalhes da localização do usuário ... 
        await using (var tx = await _unitOfWork.BeginAsync(ct))
        {
            await _geoLogRepository.InsertEnrichmentAsync(id, enriched, ct);
            await tx.CommitAsync();
        }

        return id;
    }
}
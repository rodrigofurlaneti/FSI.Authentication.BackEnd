using System;
using System.Data;
using System.Threading;
using System.Threading.Tasks;
using FSI.Authentication.Application.DTOs.Geo;
using FSI.Authentication.Application.Interfaces.Repositories;
using FSI.Authentication.Infrastructure.Persistence;
using Microsoft.Data.SqlClient;

namespace FSI.Authentication.Infrastructure.Repositories;

public sealed class GeoLogRepository : IGeoLogRepository
{
    private readonly DbSession _dbSession;
    public GeoLogRepository(DbSession dbSession) => _dbSession = dbSession;

    public async Task<long> InsertAsync(ClientContextPayload payload, CancellationToken ct)
    {
        using var cmd = new SqlCommand("dbo.usp_GeoClientLog_Insert", _dbSession.Connection, _dbSession.Transaction)
        { CommandType = CommandType.StoredProcedure };

        var g = payload.Geo;
        var e = payload.Env;
        var c = e?.Connection;

        // 2) Se sua SP aceitar CreatedAt opcional, pode comentar a linha abaixo.
        //    Se exigir, mantenha para evitar erro.
        cmd.Parameters.Add(new SqlParameter("@Lat", SqlDbType.Float) { Value = (object?)g?.Latitude ?? DBNull.Value });
        cmd.Parameters.Add(new SqlParameter("@Lon", SqlDbType.Float) { Value = (object?)g?.Longitude ?? DBNull.Value });
        cmd.Parameters.Add(new SqlParameter("@AccuracyMeters", SqlDbType.Float) { Value = (object?)g?.AccuracyMeters ?? DBNull.Value });
        cmd.Parameters.Add(new SqlParameter("@AltitudeMeters", SqlDbType.Float) { Value = (object?)g?.AltitudeMeters ?? DBNull.Value });
        cmd.Parameters.Add(new SqlParameter("@AltitudeAccuracyMeters", SqlDbType.Float) { Value = (object?)g?.AltitudeAccuracyMeters ?? DBNull.Value });
        cmd.Parameters.Add(new SqlParameter("@SpeedMps", SqlDbType.Float) { Value = (object?)g?.SpeedMetersPerSecond ?? DBNull.Value });
        cmd.Parameters.Add(new SqlParameter("@HeadingDegrees", SqlDbType.Float) { Value = (object?)g?.HeadingDegrees ?? DBNull.Value });
        cmd.Parameters.Add(new SqlParameter("@TsEpochMs", SqlDbType.BigInt) { Value = (object?)g?.TimestampEpochMs ?? DBNull.Value });
        cmd.Parameters.Add(new SqlParameter("@City", SqlDbType.NVarChar, 200) { Value = (object?)g?.City ?? DBNull.Value });
        cmd.Parameters.Add(new SqlParameter("@UserAgent", SqlDbType.NVarChar, 1024) { Value = (object?)e?.UserAgent ?? DBNull.Value });
        cmd.Parameters.Add(new SqlParameter("@Browser", SqlDbType.NVarChar, 100) { Value = (object?)e?.Browser ?? DBNull.Value });
        cmd.Parameters.Add(new SqlParameter("@BrowserVersion", SqlDbType.NVarChar, 50) { Value = (object?)e?.BrowserVersion ?? DBNull.Value });
        cmd.Parameters.Add(new SqlParameter("@OperatingSystem", SqlDbType.NVarChar, 100) { Value = (object?)e?.OperatingSystem ?? DBNull.Value });
        cmd.Parameters.Add(new SqlParameter("@OSVersion", SqlDbType.NVarChar, 50) { Value = (object?)e?.OSVersion ?? DBNull.Value });
        cmd.Parameters.Add(new SqlParameter("@Architecture", SqlDbType.NVarChar, 20) { Value = (object?)e?.Architecture ?? DBNull.Value });
        cmd.Parameters.Add(new SqlParameter("@Language", SqlDbType.NVarChar, 32) { Value = (object?)e?.Language ?? DBNull.Value });

        // protege quando e == null
        var langs = e?.Languages != null ? string.Join(",", e.Languages) : null;
        cmd.Parameters.Add(new SqlParameter("@Languages", SqlDbType.NVarChar, 512) { Value = (object?)langs ?? DBNull.Value });

        cmd.Parameters.Add(new SqlParameter("@Platform", SqlDbType.NVarChar, 128) { Value = (object?)e?.Platform ?? DBNull.Value });
        cmd.Parameters.Add(new SqlParameter("@IsOnline", SqlDbType.Bit) { Value = (object?)e?.IsOnline ?? DBNull.Value });
        cmd.Parameters.Add(new SqlParameter("@TimeZone", SqlDbType.NVarChar, 128) { Value = (object?)e?.TimeZone ?? DBNull.Value });
        cmd.Parameters.Add(new SqlParameter("@ScreenWidth", SqlDbType.Int) { Value = (object?)e?.ScreenWidth ?? DBNull.Value });
        cmd.Parameters.Add(new SqlParameter("@ScreenHeight", SqlDbType.Int) { Value = (object?)e?.ScreenHeight ?? DBNull.Value });
        cmd.Parameters.Add(new SqlParameter("@DevicePixelRatio", SqlDbType.Float) { Value = (object?)e?.DevicePixelRatio ?? DBNull.Value });
        cmd.Parameters.Add(new SqlParameter("@Referrer", SqlDbType.NVarChar, 1000) { Value = (object?)e?.Referrer ?? DBNull.Value });
        cmd.Parameters.Add(new SqlParameter("@PageUrl", SqlDbType.NVarChar, 1000) { Value = (object?)e?.PageUrl ?? DBNull.Value });

        cmd.Parameters.Add(new SqlParameter("@ConnectionEffectiveType", SqlDbType.NVarChar, 32) { Value = (object?)c?.EffectiveType ?? DBNull.Value });
        cmd.Parameters.Add(new SqlParameter("@ConnectionDownlinkMbps", SqlDbType.Float) { Value = (object?)c?.DownlinkMbps ?? DBNull.Value });
        cmd.Parameters.Add(new SqlParameter("@ConnectionRttMs", SqlDbType.Int) { Value = (object?)c?.RttMs ?? DBNull.Value });
        cmd.Parameters.Add(new SqlParameter("@ConnectionSaveData", SqlDbType.Bit) { Value = (object?)c?.SaveData ?? DBNull.Value });

        cmd.Parameters.Add(new SqlParameter("@DeviceType", SqlDbType.NVarChar, 16) { Value = (object?)e?.DeviceType ?? DBNull.Value });
        cmd.Parameters.Add(new SqlParameter("@DeviceModel", SqlDbType.NVarChar, 100) { Value = (object?)e?.DeviceModel ?? DBNull.Value });
        cmd.Parameters.Add(new SqlParameter("@TouchPoints", SqlDbType.Int) { Value = (object?)e?.TouchPoints ?? DBNull.Value });
        cmd.Parameters.Add(new SqlParameter("@IsBot", SqlDbType.Bit) { Value = (object?)e?.IsBot ?? DBNull.Value });

        // 3) Evitar NRE
        cmd.Parameters.Add(new SqlParameter("@BotName", SqlDbType.NVarChar, 64) { Value = (object?)e?.BotName ?? DBNull.Value });
        cmd.Parameters.Add(new SqlParameter("@Error", SqlDbType.NVarChar, 4000) { Value = (object?)payload.Error ?? DBNull.Value });

        // OUTPUT param para capturar o Id
        var pId = new SqlParameter("@NewId", SqlDbType.BigInt) { Direction = ParameterDirection.Output };
        cmd.Parameters.Add(pId);

        await cmd.ExecuteNonQueryAsync(ct);
        return (long)pId.Value;
    }

    public async Task InsertEnrichmentAsync(long geoClientLogId, GeoEnrichmentDto e, CancellationToken ct)
    {
        using var cmd = new SqlCommand("dbo.usp_GeoClientLogEnrichment_Insert", _dbSession.Connection, _dbSession.Transaction)
        { CommandType = CommandType.StoredProcedure };
        cmd.Parameters.AddWithValue("@GeoClientLogId", geoClientLogId);
        cmd.Parameters.AddWithValue("@Granularity", (object?)e.Granularity ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@Country", (object?)e.Country ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@UF", (object?)e.UF ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@Municipio", (object?)e.Municipio ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@CodigoMunicipioIBGE", (object?)e.CodigoMunicipioIBGE ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@Bairro", (object?)e.Bairro ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@Logradouro", (object?)e.Logradouro ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@Numero", (object?)e.Numero ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@CEP", (object?)e.CEP ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@TimezoneId", (object?)e.TimezoneId ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@GeoConfidence", (object?)e.Confidence ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@GeoSource", e.Sources is { Length: > 0 } s ? string.Join(";", s) : (object)DBNull.Value);
        cmd.Parameters.AddWithValue("@GeoAttribution", (object?)e.Attribution ?? DBNull.Value);
        await cmd.ExecuteNonQueryAsync(ct);
    }
}

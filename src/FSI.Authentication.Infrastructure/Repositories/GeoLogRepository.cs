using System.Data;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using FSI.Authentication.Application.DTOs.Geo;
using FSI.Authentication.Application.Interfaces.Repositories;
using FSI.Authentication.Infrastructure.Persistence;
using Microsoft.Data.SqlClient;

namespace FSI.Authentication.Infrastructure.Repositories;

public sealed class GeoLogRepository : IGeoLogRepository
{
    private readonly DbSession _session;
    public GeoLogRepository(DbSession session) => _session = session;

    public async Task InsertAsync(ClientContextPayload payload, CancellationToken ct)
    {
        using var cmd = new SqlCommand("dbo.usp_GeoLog_Insert", _session.Connection, _session.Transaction)
        { CommandType = CommandType.StoredProcedure };

        var g = payload.Geo;
        var e = payload.Env;
        var c = e?.Connection;

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
        cmd.Parameters.Add(new SqlParameter("@Language", SqlDbType.NVarChar, 32) { Value = (object?)e?.Language ?? DBNull.Value });
        cmd.Parameters.Add(new SqlParameter("@Languages", SqlDbType.NVarChar, 512) { Value = (object?)(e?.Languages is null ? null : string.Join(",", e.Languages)) ?? DBNull.Value });
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

        cmd.Parameters.Add(new SqlParameter("@Error", SqlDbType.NVarChar, 4000) { Value = (object?)payload.Error ?? DBNull.Value });

        await cmd.ExecuteNonQueryAsync(ct);
    }
}

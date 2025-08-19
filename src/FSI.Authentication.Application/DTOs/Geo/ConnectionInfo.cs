using System.Text.Json.Serialization;

namespace FSI.Authentication.Application.DTOs.Geo
{
    public sealed class ConnectionInfo
    {
        [JsonPropertyName("effectiveType")]
        public string? EffectiveType { get; init; }
        [JsonPropertyName("downlink")]
        public double? DownlinkMbps { get; init; }
        [JsonPropertyName("rtt")]
        public int? RttMs { get; init; }
        [JsonPropertyName("saveData")]
        public bool? SaveData { get; init; }
    }
}

using System.Text.Json.Serialization;

namespace FSI.Authentication.Application.DTOs.Geo
{
    public sealed class ClientContextPayload
    {
        [JsonPropertyName("geo")]
        public GeoData? Geo { get; init; }
        [JsonPropertyName("env")]
        public EnvData Env { get; init; } = new();
        [JsonPropertyName("error")]
        public string? Error { get; init; }
    }
}
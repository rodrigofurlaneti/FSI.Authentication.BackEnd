using System.Text.Json.Serialization;

namespace FSI.Authentication.Application.ExternalModels.Geo
{
    public sealed class NominatimResponse
    {
        [JsonPropertyName("lat")] public string? Lat { get; set; }
        [JsonPropertyName("lon")] public string? Lon { get; set; }
        [JsonPropertyName("address")] public NominatimAddress? Address { get; set; }
    }
}

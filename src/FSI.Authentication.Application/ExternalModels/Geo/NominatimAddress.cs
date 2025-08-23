using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace FSI.Authentication.Application.ExternalModels.Geo
{
    public sealed class NominatimAddress
    {
        [JsonPropertyName("country_code")] public string? CountryCode { get; set; }
        [JsonPropertyName("state")] public string? State { get; set; }
        [JsonPropertyName("state_code")] public string? StateCode { get; set; } // Nem sempre vem
        [JsonPropertyName("city")] public string? City { get; set; }
        [JsonPropertyName("town")] public string? Town { get; set; }
        [JsonPropertyName("village")] public string? Village { get; set; }
        [JsonPropertyName("municipality")] public string? Municipality { get; set; }
        [JsonPropertyName("county")] public string? County { get; set; }
        [JsonPropertyName("suburb")] public string? Suburb { get; set; }
        [JsonPropertyName("neighbourhood")] public string? Neighbourhood { get; set; }
        [JsonPropertyName("borough")] public string? Borough { get; set; }
        [JsonPropertyName("road")] public string? Road { get; set; }
        [JsonPropertyName("house_number")] public string? HouseNumber { get; set; }
        [JsonPropertyName("postcode")] public string? Postcode { get; set; }
    }
}

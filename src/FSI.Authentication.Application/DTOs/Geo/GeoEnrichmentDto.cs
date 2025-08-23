using System.Text.Json.Serialization;

namespace FSI.Authentication.Application.DTOs.Geo
{
    public sealed class GeoEnrichmentDto
    {
        [JsonPropertyName("granularity")]
        public string? Granularity { get; set; }

        [JsonPropertyName("country")]
        public string? Country { get; set; }

        [JsonPropertyName("uf")]
        public string? UF { get; set; }

        [JsonPropertyName("municipio")]
        public string? Municipio { get; set; }

        [JsonPropertyName("codigo_municipio_ibge")]
        public string? CodigoMunicipioIBGE { get; set; }

        [JsonPropertyName("bairro")]
        public string? Bairro { get; set; }

        [JsonPropertyName("logradouro")]
        public string? Logradouro { get; set; }

        [JsonPropertyName("numero")]
        public string? Numero { get; set; }

        [JsonPropertyName("cep")]
        public string? CEP { get; set; }

        [JsonPropertyName("timezone")]
        public string? TimezoneId { get; set; }

        [JsonPropertyName("confidence")]
        public double? Confidence { get; set; }

        [JsonPropertyName("sources")]
        public string[]? Sources { get; set; }

        [JsonPropertyName("attribution")]
        public string? Attribution { get; set; }
    }
}

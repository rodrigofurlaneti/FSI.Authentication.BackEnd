using System.Text.Json.Serialization;

namespace FSI.Authentication.Application.DTOs.Geo
{
    public sealed class GeoData
    {
        [JsonPropertyName("lat")]
        public double? Latitude { get; init; }
        [JsonPropertyName("lon")]
        public double? Longitude { get; init; }
        [JsonPropertyName("accuracy")]
        public double? AccuracyMeters { get; init; }
        [JsonPropertyName("altitude")]
        public double? AltitudeMeters { get; init; }
        [JsonPropertyName("altitudeAccuracy")]
        public double? AltitudeAccuracyMeters { get; init; }
        [JsonPropertyName("speed")]
        public double? SpeedMetersPerSecond { get; init; }
        [JsonPropertyName("heading")]
        public double? HeadingDegrees { get; init; }
        [JsonPropertyName("ts")]
        public long? TimestampEpochMs { get; init; }
        [JsonPropertyName("city")]
        public string? City { get; init; }
    }
}

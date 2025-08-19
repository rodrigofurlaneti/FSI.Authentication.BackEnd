using System.Text.Json.Serialization;

namespace FSI.Authentication.Application.DTOs.Geo
{
    public sealed class EnvData
    {
        [JsonPropertyName("ua")]
        public string? UserAgent { get; init; }
        [JsonPropertyName("browser")]
        public string? Browser { get; init; }
        [JsonPropertyName("browserVersion")]
        public string? BrowserVersion { get; init; }
        [JsonPropertyName("operatingSystem")]
        public string? OperatingSystem { get; init; }
        [JsonPropertyName("osVersion")]
        public string? OSVersion { get; init; }
        [JsonPropertyName("architecture")]
        public string? Architecture { get; init; }
        [JsonPropertyName("deviceType")]
        public string? DeviceType { get; init; }
        [JsonPropertyName("deviceModel")]
        public string? DeviceModel { get; init; }
        [JsonPropertyName("touchPoints")]
        public int? TouchPoints { get; init; }
        [JsonPropertyName("isBot")]
        public bool? IsBot { get; init; }
        [JsonPropertyName("botName")]
        public string? BotName { get; init; }
        [JsonPropertyName("language")]
        public string? Language { get; init; }
        [JsonPropertyName("languages")]
        public string[]? Languages { get; init; }
        [JsonPropertyName("platform")]
        public string? Platform { get; init; }
        [JsonPropertyName("online")]
        public bool? IsOnline { get; init; }
        [JsonPropertyName("timeZone")]
        public string? TimeZone { get; init; }
        [JsonPropertyName("screenWidth")]
        public int? ScreenWidth { get; init; }
        [JsonPropertyName("screenHeight")]
        public int? ScreenHeight { get; init; }
        [JsonPropertyName("dpr")]
        public double? DevicePixelRatio { get; init; }
        [JsonPropertyName("referrer")]
        public string? Referrer { get; init; }
        [JsonPropertyName("page")]
        public string? PageUrl { get; init; }
        [JsonPropertyName("connection")]
        public ConnectionInfo? Connection { get; init; }
    }
}

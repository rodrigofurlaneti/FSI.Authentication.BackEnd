using FSI.Authentication.Application.DTOs.Geo;
using FSI.Authentication.Application.ExternalModels.Geo;
using FSI.Authentication.Application.Interfaces.Services;

// se for usar a extensão:
using FSI.Authentication.Application.Shared.Extensions;

using System;
using System.Collections.Concurrent;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;

namespace FSI.Authentication.Application.Services
{
    public sealed class GeoEnricherService : IGeoEnricher
    {
        private readonly HttpClient _http;
        private static readonly JsonSerializerOptions JsonOpts = new(JsonSerializerDefaults.Web)
        {
            PropertyNameCaseInsensitive = true
        };

        private static readonly ConcurrentDictionary<string, string> IbgeCache = new();

        public GeoEnricherService(HttpClient http)
        {
            _http = http ?? throw new ArgumentNullException(nameof(http));
            _http.DefaultRequestHeaders.UserAgent.ParseAdd("FurlanetiGeoEnricher/1.0 (+contato@furlaneti.com)");
        }

        public async Task<GeoEnrichmentDto> EnrichAsync(ClientContextPayload payload, CancellationToken ct)
        {
            var g = payload.Geo;
            var e = payload.Env;

            if (g?.Latitude is null || g.Longitude is null)
                return Empty("coordenadas ausentes");

            var lat = g.Latitude.Value;
            var lon = g.Longitude.Value;
            if (lat is < -90 or > 90 || lon is < -180 or > 180)
                return Empty("coordenadas inválidas");

            var gran = ResolveGranularity(g.AccuracyMeters);

            NominatimResponse? nomi = null;
            try { nomi = await ReverseNominatimAsync(lat, lon, ct); }
            catch { /* log opcional */ }

            var addr = nomi?.Address;
            var countryCode = addr?.CountryCode?.ToUpperInvariant();
            var uf = NormalizeUF(addr);
            var municipio = addr?.City ?? addr?.Town ?? addr?.Village ?? addr?.Municipality ?? addr?.County;

            string? codIbge = null;
            if (string.Equals(countryCode, "BR", StringComparison.OrdinalIgnoreCase) &&
                !string.IsNullOrWhiteSpace(uf) && !string.IsNullOrWhiteSpace(municipio))
            {
                codIbge = await ResolveIbgeAsync(uf!, municipio!, ct);
            }

            var cep = addr?.Postcode;
            if (!string.IsNullOrWhiteSpace(cep))
                cep = OnlyDigits(cep);

            var tz = !string.IsNullOrWhiteSpace(e?.TimeZone) ? e!.TimeZone : ResolveTimezoneHeuristic(countryCode, lat, lon);

            var logradouro = addr?.Road;
            var numero = addr?.HouseNumber;
            if (gran is "bairro" or "cidade") { logradouro = null; numero = null; }
            else if (gran is "quarteirao") { numero = null; }

            var confidence = ComputeConfidence(g.AccuracyMeters, nomi != null, codIbge != null);

            // Se NÃO chamar ViaCEP de fato, NÃO liste "viacep" nas fontes.
            // Se preferir manter a extensão:
            var sources = new[] { "nominatim", countryCode == "BR" ? "br:ibge" : null }
                          .Where(s => !string.IsNullOrWhiteSpace(s))
                          .ToArray();
            // ou: .WhereNotNull().ToArray();  // usando a extensão

            var dto = new GeoEnrichmentDto
            {
                Granularity = gran,
                Country = countryCode,
                UF = uf,
                Municipio = ToTitleCase(municipio),
                CodigoMunicipioIBGE = codIbge,
                Bairro = ToTitleCase(addr?.Suburb ?? addr?.Neighbourhood ?? addr?.Borough),
                Logradouro = ToTitleCase(logradouro),
                Numero = numero,
                CEP = cep,
                TimezoneId = tz,
                Confidence = confidence,
                Sources = sources,
                Attribution = "© OpenStreetMap contributors"
            };

            return dto;
        }

        private static string ResolveGranularity(double? acc) =>
            acc switch
            {
                <= 30 => "rua",
                <= 100 => "quarteirao",
                <= 1000 => "bairro",
                _ => "cidade"
            };

        private async Task<NominatimResponse?> ReverseNominatimAsync(double lat, double lon, CancellationToken ct)
        {
            var url = $"https://nominatim.openstreetmap.org/reverse?format=jsonv2&lat={lat.ToString(CultureInfo.InvariantCulture)}&lon={lon.ToString(CultureInfo.InvariantCulture)}&addressdetails=1";
            using var req = new HttpRequestMessage(HttpMethod.Get, url);
            var rsp = await _http.SendAsync(req, ct);
            if (!rsp.IsSuccessStatusCode) return null;
            var json = await rsp.Content.ReadAsStringAsync(ct);
            return JsonSerializer.Deserialize<NominatimResponse>(json, JsonOpts);
        }

        private static string? NormalizeUF(NominatimAddress? a)
        {
            if (a is null) return null;
            if (!string.IsNullOrWhiteSpace(a.StateCode))
                return a.StateCode.ToUpperInvariant();
            // TODO: fallback mapear a.State -> sigla (SP/RJ/…)
            return null;
        }

        private static string ResolveTimezoneHeuristic(string? countryCode, double lat, double lon) =>
            string.Equals(countryCode, "BR", StringComparison.OrdinalIgnoreCase)
                ? "America/Sao_Paulo"
                : "UTC";

        private static string? OnlyDigits(string? s)
        {
            if (string.IsNullOrWhiteSpace(s)) return s;
            Span<char> buf = stackalloc char[s.Length];
            var j = 0;
            foreach (var ch in s)
                if (char.IsDigit(ch)) buf[j++] = ch;
            return new string(buf[..j]);
        }

        private static string? ToTitleCase(string? s)
        {
            if (string.IsNullOrWhiteSpace(s)) return s;
            return CultureInfo.GetCultureInfo("pt-BR").TextInfo.ToTitleCase(s.ToLowerInvariant());
        }

        private static double ComputeConfidence(double? acc, bool hasNom, bool hasIbge)
        {
            double c = 0.4 + (hasNom ? 0.3 : 0) + (hasIbge ? 0.2 : 0);
            if (acc is > 0 and <= 100) c += 0.1;
            if (acc is > 1000) c -= 0.2;
            return Math.Clamp(c, 0.1, 0.95);
        }

        private async Task<string?> ResolveIbgeAsync(string uf, string municipio, CancellationToken ct)
        {
            var key = $"{uf}|{municipio}".ToUpperInvariant().Normalize(NormalizationForm.FormD);
            if (IbgeCache.TryGetValue(key, out var code)) return code;

            var url = $"https://servicodados.ibge.gov.br/api/v1/localidades/estados/{uf}/municipios";
            using var req = new HttpRequestMessage(HttpMethod.Get, url);
            using var rsp = await _http.SendAsync(req, ct);
            if (!rsp.IsSuccessStatusCode) return null;

            var body = await rsp.Content.ReadAsStringAsync(ct);
            using var doc = JsonDocument.Parse(body);
            foreach (var m in doc.RootElement.EnumerateArray())
            {
                var nome = m.GetProperty("nome").GetString();
                if (StringEqualsIgnoreAccents(nome, municipio))
                {
                    var id = m.GetProperty("id").GetInt32();
                    var str = id.ToString("0000000", CultureInfo.InvariantCulture);
                    IbgeCache[key] = str;
                    return str;
                }
            }
            return null;

            static bool StringEqualsIgnoreAccents(string? a, string? b)
            {
                if (string.IsNullOrWhiteSpace(a) || string.IsNullOrWhiteSpace(b)) return false;
                return RemoveDiacritics(a).Equals(RemoveDiacritics(b), StringComparison.OrdinalIgnoreCase);
            }

            static string RemoveDiacritics(string text)
            {
                var formD = text.Normalize(NormalizationForm.FormD);
                Span<char> outBuf = stackalloc char[formD.Length];
                var j = 0;
                foreach (var ch in formD)
                {
                    var uc = System.Globalization.CharUnicodeInfo.GetUnicodeCategory(ch);
                    if (uc != System.Globalization.UnicodeCategory.NonSpacingMark)
                        outBuf[j++] = ch;
                }
                return new string(outBuf[..j]).Normalize(NormalizationForm.FormC);
            }
        }

        private static GeoEnrichmentDto Empty(string reason) => new()
        {
            Granularity = "cidade",
            Confidence = 0.1,
            Sources = Array.Empty<string>(),
            Attribution = "© OpenStreetMap contributors"
        };
    }
}

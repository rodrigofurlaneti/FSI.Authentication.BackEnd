namespace FSI.Authentication.Infrastructure.Security
{
    public sealed class JwtOptions
    {
        public string Issuer { get; init; } = "FSI.Auth";
        public string Audience { get; init; } = "FSI.API";
        public string SigningKey { get; init; } = default!; // chave simétrica base64/hex/plain
        public int ExpirationMinutes { get; init; } = 60;
    }
}

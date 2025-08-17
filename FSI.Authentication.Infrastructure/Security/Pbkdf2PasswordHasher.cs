using System.Security.Cryptography;
using System.Text;
using System.Globalization;
using AppServ = FSI.Authentication.Application.Interfaces.Services;

namespace FSI.Authentication.Infrastructure.Security
{
    /// <summary>
    /// PBKDF2-HMACSHA256 com suporte a dois formatos:
    /// - "$PBKDF2$HMACSHA256$<iter>$<saltB64>$<hashB64>"  (formato usado no seu seed)
    /// - "<iter>.<saltB64>.<hashB64>"                     (formato antigo/simples)
    /// </summary>
    public sealed class Pbkdf2PasswordHasher : AppServ.IPasswordHasher
    {
        private const int IterationsDefault = 100_000;
        private const int SaltSize = 16;
        private const int KeySize = 32;

        public string Hash(string rawPassword)
        {
            if (string.IsNullOrWhiteSpace(rawPassword))
                throw new ArgumentException("Senha vazia.", nameof(rawPassword));

            var salt = RandomNumberGenerator.GetBytes(SaltSize);

            var key = Rfc2898DeriveBytes.Pbkdf2(
                Encoding.UTF8.GetBytes(rawPassword),
                salt,
                IterationsDefault,
                HashAlgorithmName.SHA256,
                KeySize);

            // Emite no mesmo formato do seu banco
            return $"$PBKDF2$HMACSHA256${IterationsDefault.ToString(CultureInfo.InvariantCulture)}${Convert.ToBase64String(salt)}${Convert.ToBase64String(key)}";
        }

        public bool Verify(string rawPassword, string passwordHash)
        {
            if (string.IsNullOrEmpty(passwordHash) || string.IsNullOrEmpty(rawPassword))
                return false;

            // Formato "$PBKDF2$HMACSHA256$<iter>$<saltB64>$<hashB64>"
            if (passwordHash.StartsWith("$PBKDF2$", StringComparison.OrdinalIgnoreCase))
            {
                var parts = passwordHash.Split('$', StringSplitOptions.RemoveEmptyEntries);
                // Esperado: ["PBKDF2","HMACSHA256","<iter>","<saltB64>","<hashB64>"]
                if (parts.Length != 5) return false;
                if (!parts[1].Equals("HMACSHA256", StringComparison.OrdinalIgnoreCase)) return false;

                if (!int.TryParse(parts[2], NumberStyles.Integer, CultureInfo.InvariantCulture, out var iter) || iter <= 0)
                    return false;

                byte[] salt, expected;
                try
                {
                    salt = Convert.FromBase64String(parts[3]);
                    expected = Convert.FromBase64String(parts[4]);
                }
                catch { return false; }

                var calc = Rfc2898DeriveBytes.Pbkdf2(
                    Encoding.UTF8.GetBytes(rawPassword),
                    salt,
                    iter,
                    HashAlgorithmName.SHA256,
                    expected.Length);

                return CryptographicOperations.FixedTimeEquals(calc, expected);
            }

            // Formato "<iter>.<saltB64>.<hashB64>"
            var dotParts = passwordHash.Split('.', StringSplitOptions.RemoveEmptyEntries);
            if (dotParts.Length == 3)
            {
                if (!int.TryParse(dotParts[0], NumberStyles.Integer, CultureInfo.InvariantCulture, out var iter) || iter <= 0)
                    return false;

                byte[] salt, expected;
                try
                {
                    salt = Convert.FromBase64String(dotParts[1]);
                    expected = Convert.FromBase64String(dotParts[2]);
                }
                catch { return false; }

                var calc = Rfc2898DeriveBytes.Pbkdf2(
                    Encoding.UTF8.GetBytes(rawPassword),
                    salt,
                    iter,
                    HashAlgorithmName.SHA256,
                    expected.Length);

                return CryptographicOperations.FixedTimeEquals(calc, expected);
            }

            // Formato desconhecido
            return false;
        }
    }
}

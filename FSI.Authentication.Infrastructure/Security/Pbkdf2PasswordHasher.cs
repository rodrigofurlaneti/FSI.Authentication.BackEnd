using AppServ = FSI.Authentication.Application.Interfaces.Services;
using System.Security.Cryptography;
using System.Text;

namespace FSI.Authentication.Infrastructure.Security
{
    /// <summary>
    /// Implementação simples baseada em PBKDF2 (sem pacote externo).
    /// Ajuste parâmetros conforme sua política de segurança.
    /// </summary>
    public sealed class Pbkdf2PasswordHasher : AppServ.IPasswordHasher
    {
        private const int Iterations = 100_000;
        private const int SaltSize = 16;
        private const int KeySize = 32;

        public string Hash(string rawPassword)
        {
            using var salt = RandomNumberGenerator.Create();
            var saltBytes = new byte[SaltSize];
            salt.GetBytes(saltBytes);

            var key = Rfc2898DeriveBytes.Pbkdf2(
                Encoding.UTF8.GetBytes(rawPassword),
                saltBytes,
                Iterations,
                HashAlgorithmName.SHA256,
                KeySize);

            return $"{Iterations}.{Convert.ToBase64String(saltBytes)}.{Convert.ToBase64String(key)}";
        }

        public bool Verify(string rawPassword, string passwordHash)
        {
            var parts = passwordHash.Split('.');
            if (parts.Length != 3) return false;

            var iterations = int.Parse(parts[0]);
            var salt = Convert.FromBase64String(parts[1]);
            var key = Convert.FromBase64String(parts[2]);

            var calc = Rfc2898DeriveBytes.Pbkdf2(
                Encoding.UTF8.GetBytes(rawPassword),
                salt,
                iterations,
                HashAlgorithmName.SHA256,
                key.Length);

            return CryptographicOperations.FixedTimeEquals(calc, key);
        }
    }
}

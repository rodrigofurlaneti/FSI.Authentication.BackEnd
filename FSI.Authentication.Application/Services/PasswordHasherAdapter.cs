using System;
using FSI.Authentication.Application.Interfaces.Services;

namespace FSI.Authentication.Application.Services
{
    /// <summary>
    /// Adapter opcional. Não use em produção; a implementação real vem da camada Infra (Pbkdf2PasswordHasher).
    /// </summary>
    public sealed class PasswordHasherAdapter : IPasswordHasher
    {
        public string Hash(string password) =>
            throw new NotSupportedException("Use Infrastructure.Security.Pbkdf2PasswordHasher via DI.");

        public bool Verify(string password, string hash) =>
            throw new NotSupportedException("Use Infrastructure.Security.Pbkdf2PasswordHasher via DI.");
    }
}

using FSI.Authentication.Domain.Abstractions;
using FSI.Authentication.Domain.Aggregates;

namespace FSI.Authentication.Domain.Services
{
    public sealed class AuthDomainService
    {
        private readonly IPasswordPolicy passwordPolicy;
        private readonly IClock clock;
        public AuthDomainService(IPasswordPolicy passwordPolicy, IClock clock) { this.passwordPolicy = passwordPolicy; this.clock = clock; }
        public Result EnsurePasswordStrength(string rawPassword) => passwordPolicy.IsStrong(rawPassword, out var r) ? Result.Success() : Result.Failure(r ?? "Senha fraca");
        public bool CanSignIn(UserAccount u) => u.CanSignIn(clock);
    }
}

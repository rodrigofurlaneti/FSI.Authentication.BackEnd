using System.Threading;
using System.Threading.Tasks;
using FSI.Authentication.Domain.Interfaces;
using FSI.Authentication.Domain.Aggregates;
using FSI.Authentication.Domain.Abstractions;

namespace FSI.Authentication.Infrastructure.Services
{
    public sealed class ProfileService : IProfileService
    {
        private readonly IClock clock;

        public ProfileService(IClock clock) => this.clock = clock;

        public Task<UserAccount?> GetByEmailAsync(string email, CancellationToken ct = default)
        {
            // TODO: trocar por repositório/EF
            // Stub: retorna um usuário fake
            var user = new UserAccount(
                Guid.NewGuid(), email, "John", "Doe",
                "HASH", "User", isActive: true, failedLoginCount: 0, lockoutEndUtc: null
            );
            return Task.FromResult<UserAccount?>(user);
        }

        public Task<UserAccount> ChangeProfileAsync(string email, string newProfileName, CancellationToken ct = default)
        {
            var user = new UserAccount(
                Guid.NewGuid(), email, "John", "Doe",
                "HASH", "User", isActive: true, failedLoginCount: 0, lockoutEndUtc: null
            );
            user.ChangeProfile(newProfileName, clock);
            return Task.FromResult(user);
        }
    }
}

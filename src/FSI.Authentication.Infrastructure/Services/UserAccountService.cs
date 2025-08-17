using FSI.Authentication.Domain.Abstractions;
using FSI.Authentication.Domain.Aggregates;
using FSI.Authentication.Domain.Interfaces;
using FSI.Authentication.Domain.Services;
using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FSI.Authentication.Infrastructure.Services
{
    public sealed class UserAccountService : IUserAccountService, IAuthDomainService
    {
        private readonly IClock clock;

        public UserAccountService(IClock clock) => this.clock = clock;

        public Task<UserAccount?> GetByEmailAsync(string email, CancellationToken ct = default)
        {
            // TODO: substituir por acesso a banco
            var user = new UserAccount(Guid.NewGuid(), email, "John", "Doe", "HASH", "User");
            return Task.FromResult<UserAccount?>(user);
        }

        public Task SaveAsync(UserAccount user, CancellationToken ct = default)
        {
            // TODO: persistir (EF/Dapper/etc). Aqui é só um stub.
            return Task.CompletedTask;
        }

        public bool CanSignIn(UserAccount user, IClock clock, out string? reason) => user.CanSignIn(clock, out reason);
    }
}
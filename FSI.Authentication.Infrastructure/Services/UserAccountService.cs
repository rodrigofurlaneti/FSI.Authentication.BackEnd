using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using FSI.Authentication.Domain.Aggregates;
using FSI.Authentication.Domain.Interfaces;
using FSI.Authentication.Domain.Abstractions;

namespace FSI.Authentication.Infrastructure.Services
{
    public sealed class UserAccountService : IUserAccountService
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
    }
}
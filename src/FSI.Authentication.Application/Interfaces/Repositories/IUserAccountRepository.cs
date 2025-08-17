using System.Threading;
using System.Threading.Tasks;
using FSI.Authentication.Domain.Aggregates;
using FSI.Authentication.Domain.ValueObjects;

namespace FSI.Authentication.Application.Interfaces.Repositories
{
    public interface IUserAccountRepository
    {
        Task<UserAccount?> GetByEmailAsync(Email email, CancellationToken ct);
        Task AddAsync(UserAccount user, CancellationToken ct);
        Task UpdateAsync(UserAccount user, CancellationToken ct);
    }
}

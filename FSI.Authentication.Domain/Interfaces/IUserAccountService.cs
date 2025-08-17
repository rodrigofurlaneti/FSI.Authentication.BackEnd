using System.Threading;
using System.Threading.Tasks;
using FSI.Authentication.Domain.Aggregates;

namespace FSI.Authentication.Domain.Interfaces
{
    public interface IUserAccountService
    {
        Task<UserAccount?> GetByEmailAsync(string email, CancellationToken ct = default);
        Task SaveAsync(UserAccount user, CancellationToken ct = default);
    }
}
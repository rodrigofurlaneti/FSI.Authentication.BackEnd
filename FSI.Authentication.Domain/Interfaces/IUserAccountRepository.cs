using FSI.Authentication.Domain.Aggregates;

namespace FSI.Authentication.Domain.Interfaces
{
    public interface IUserAccountRepository
    {
        Task<UserAccount?> GetByEmailAsync(string email, CancellationToken ct = default);
        Task<UserAccount?> GetByIdAsync(Guid id, CancellationToken ct = default);
        Task AddAsync(UserAccount user, CancellationToken ct = default);
        Task UpdateAsync(UserAccount user, CancellationToken ct = default);
        Task SaveChangesAsync(CancellationToken ct = default);
    }
}

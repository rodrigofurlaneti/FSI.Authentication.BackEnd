using FSI.Authentication.Domain.Aggregates;
using System.Threading;
using System.Threading.Tasks;

namespace FSI.Authentication.Domain.Interfaces
{
    public interface IProfileService
    {
        Task<UserAccount?> GetByEmailAsync(string email, CancellationToken ct = default);
        Task<UserAccount> ChangeProfileAsync(string email, string newProfileName, CancellationToken ct = default);
    }

    // Entidade/VO de domínio mínima para destravar:
    public sealed record Profile(string Email, string ProfileName);
}

using FSI.Authentication.Domain.Abstractions;
using FSI.Authentication.Domain.Aggregates;

namespace FSI.Authentication.Domain.Services
{
    public interface IAuthDomainService
    {
        bool CanSignIn(UserAccount user, IClock clock, out string? reason);
    }
}

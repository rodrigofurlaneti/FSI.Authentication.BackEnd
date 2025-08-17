using FSI.Authentication.Domain.Aggregates;

namespace FSI.Authentication.Domain.Abstractions.Security
{
    public interface ITokenProvider
    {
        string CreateAccessToken(UserAccount user);
        string CreateRefreshToken(UserAccount user);
    }
}

using System;
using FSI.Authentication.Domain.Aggregates;

namespace FSI.Authentication.Application.Interfaces.Services
{
    public sealed record AccessToken(string Token, DateTimeOffset ExpiresAtUtc);

    public interface ITokenProvider
    {
        AccessToken CreateToken(UserAccount user);
    }
}

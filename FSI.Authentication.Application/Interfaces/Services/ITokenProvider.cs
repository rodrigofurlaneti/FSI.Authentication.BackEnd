using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSI.Authentication.Application.Interfaces.Services
{
    public interface ITokenProvider
    {
        (string accessToken, DateTime expiresAtUtc) CreateAccessToken(
            Guid userId, string email, string profile, IEnumerable<string> permissions);

        string CreateRefreshToken(Guid userId);
    }
}

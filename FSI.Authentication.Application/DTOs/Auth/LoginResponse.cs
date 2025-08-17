using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSI.Authentication.Application.DTOs.Auth
{
    public sealed record LoginResponse(
        string AccessToken,
        string RefreshToken,
        DateTime ExpiresAtUtc,
        string Profile,
        string[] Permissions);
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSI.Authentication.Application.DTOs.Auth
{
    public sealed record RegisterUserResponse(
        Guid UserId,
        string Email,
        string Name,
        string Profile);
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSI.Authentication.Application.DTOs.Shared
{
    public sealed record UserProfileDto(
        Guid UserId,
        string Email,
        string Name,
        bool IsActive,
        ProfileDto Profile);
}

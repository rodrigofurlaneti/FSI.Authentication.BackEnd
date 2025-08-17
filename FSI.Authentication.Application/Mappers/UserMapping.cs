using FSI.Authentication.Application.DTOs.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSI.Authentication.Application.Mappers
{
    public static class UserMapping
    {
        public static UserProfileDto ToProfileDto(UserAccount u)
            => new(
                u.Id,
                u.Email.Value,
                u.Name.ToString(),
                u.IsActive,
                new ProfileDto(
                    u.Profile.Name.Value,
                    u.Profile.Permissions.Select(p => new PermissionDto(p.Code, p.Description)).ToArray()
                )
            );
    }

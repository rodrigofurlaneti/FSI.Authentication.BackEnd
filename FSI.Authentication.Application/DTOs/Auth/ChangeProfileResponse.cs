using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSI.Authentication.Application.DTOs.Auth
{
    public sealed record ChangeProfileResponse(
        Guid UserId,
        string OldProfile,
        string NewProfile);
}

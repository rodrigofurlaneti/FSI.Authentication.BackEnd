using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSI.Authentication.Application.DTOs.Shared
{
    public sealed record PermissionDto(string Code, string? Description);
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSI.Authentication.Application.DTOs.Auth
{
    public sealed record RegisterUserRequest(
        string Email,
        string FirstName,
        string? LastName,
        string Password,
        string Profile);

}

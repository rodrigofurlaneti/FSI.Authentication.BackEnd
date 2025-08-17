using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSI.Authentication.Application.UseCases.RegisterUser
{
    public sealed record RegisterUserCommand(
        string Email,
        string FirstName,
        string? LastName,
        string Password,
        string ProfileName
    );
}

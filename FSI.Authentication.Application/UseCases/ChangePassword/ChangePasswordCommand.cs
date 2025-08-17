using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSI.Authentication.Application.UseCases.ChangePassword
{
    public sealed record ChangePasswordCommand(string Email, string CurrentPassword, string NewPassword);

}

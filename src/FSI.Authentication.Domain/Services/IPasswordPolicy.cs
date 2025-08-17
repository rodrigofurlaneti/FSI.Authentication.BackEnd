using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSI.Authentication.Domain.Services
{
    public interface IPasswordPolicy { bool IsStrong(string rawPassword, out string? reason); }
}

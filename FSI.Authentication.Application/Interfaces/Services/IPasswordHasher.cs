using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSI.Authentication.Application.Interfaces.Services
{
    public interface IPasswordHasher
    {
        string Hash(string rawPassword);
        bool Verify(string rawPassword, string passwordHash);
    }
}

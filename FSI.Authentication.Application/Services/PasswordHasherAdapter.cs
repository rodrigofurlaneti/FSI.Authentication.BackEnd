using FSI.Authentication.Application.Interfaces.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSI.Authentication.Application.Services
{
    public sealed class PasswordHasherAdapter
    {
        private readonly IPasswordHasher hasher;
        public PasswordHasherAdapter(IPasswordHasher hasher) => this.hasher = hasher;
        public PasswordHash HashToVo(string rawPassword) => PasswordHash.FromHashed(hasher.Hash(rawPassword));
    }
}

using FSI.Authentication.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSI.Authentication.Domain.Specifications
{
    public static class UserByEmailSpec
    {
        public static Func<FSI.Authentication.Domain.Aggregates.UserAccount, bool> Is(Email email) => u => u.Email == email;
    }
}

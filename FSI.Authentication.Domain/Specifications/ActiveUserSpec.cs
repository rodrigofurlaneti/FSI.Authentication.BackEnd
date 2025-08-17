using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSI.Authentication.Domain.Specifications
{
    public static class ActiveUserSpec
    {
        public static Func<FSI.Authentication.Domain.Aggregates.UserAccount, bool> IsActive() => u => u.IsActive;
    }
}

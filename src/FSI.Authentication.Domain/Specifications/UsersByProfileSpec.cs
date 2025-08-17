using System;
using System.Linq.Expressions;
using FSI.Authentication.Domain.Aggregates;

namespace FSI.Authentication.Domain.Specifications
{
    // Assume que você já tem uma base Specification<T> que recebe uma expressão.
    public sealed class UsersByProfileSpec : Specification<UserAccount>
    {
        public UsersByProfileSpec(string profileName)
            : base(HasProfile(profileName))
        {
        }

        private static Expression<Func<UserAccount, bool>> HasProfile(string profileName)
            => u => u.ProfileName == profileName; // ajuste para string
    }
}

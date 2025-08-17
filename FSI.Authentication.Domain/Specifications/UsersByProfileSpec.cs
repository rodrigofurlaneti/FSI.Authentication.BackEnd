using FSI.Authentication.Domain.ValueObjects;

namespace FSI.Authentication.Domain.Specifications
{
    public static class UsersByProfileSpec
    {
        public static Func<FSI.Authentication.Domain.Aggregates.UserAccount, bool> Has(ProfileName profile) => u => u.Profile.Name == profile;
    }
}

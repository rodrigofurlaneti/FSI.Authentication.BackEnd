using System;
using System.Linq.Expressions;
using FSI.Authentication.Domain.Aggregates;

namespace FSI.Authentication.Domain.Specifications
{
    public sealed class UserByEmailSpec : Specification<UserAccount>
    {
        // Comparação direta com string, pois UserAccount.Email é string
        public UserByEmailSpec(string email)
            : base(u => u.Email == email)
        { }

        // Sobrecarga para VO Email -> delega para a versão string
        public UserByEmailSpec(FSI.Authentication.Domain.ValueObjects.Email email)
            : this(email.Value)
        { }
    }
}

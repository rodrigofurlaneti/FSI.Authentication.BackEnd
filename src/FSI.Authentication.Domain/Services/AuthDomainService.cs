using FSI.Authentication.Domain.Abstractions;
using FSI.Authentication.Domain.Aggregates;

namespace FSI.Authentication.Domain.Services
{
    public sealed class AuthDomainService : IAuthDomainService
    {
        // NENHUM membro com o nome "AuthDomainService" aqui dentro!
        // (Construtor correto não tem tipo de retorno)
        public AuthDomainService() { }

        public bool CanSignIn(UserAccount user, IClock clock, out string? reason)
            => user.CanSignIn(clock, out reason);
    }
}

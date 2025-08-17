using System.Threading;
using System.Threading.Tasks;
using FSI.Authentication.Application.Exceptions;
using FSI.Authentication.Application.Interfaces.Repositories;

namespace FSI.Authentication.Application.UseCases.GetProfile
{
    public sealed class GetProfileHandler
    {
        private readonly IUserAccountRepository _users;

        public GetProfileHandler(IUserAccountRepository users)
        {
            _users = users;
        }

        public async Task<ProfileDto> Handle(GetProfileQuery query, CancellationToken ct)
        {
            var emailVo = new FSI.Authentication.Domain.ValueObjects.Email(query.Email);
            var user = await _users.GetByEmailAsync(emailVo, ct);
            if (user is null) throw new NotFoundException("Usuário não encontrado.");

            return new ProfileDto(user.Email, user.FirstName, user.LastName, user.ProfileName, user.IsActive);
        }
    }
}
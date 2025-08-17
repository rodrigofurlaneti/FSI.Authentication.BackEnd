using FSI.Authentication.Application.Interfaces.Repositories;
using FSI.Authentication.Domain.ValueObjects;

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
            var emailVo = new Email(query.Email);
   
            var user = await _users.GetByEmailAsync(emailVo, ct);
            if (user is null)
            {
                throw new System.InvalidOperationException("Usuario nao encontrado.");
            }

            return new ProfileDto(
                Email: user.Email,
                FirstName: user.FirstName,
                LastName: user.LastName,
                ProfileName: user.ProfileName,
                IsActive: user.IsActive
            );
        }
    }
}

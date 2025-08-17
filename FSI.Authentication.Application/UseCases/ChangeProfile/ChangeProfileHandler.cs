using FSI.Authentication.Application.DTOs.Auth;
using FSI.Authentication.Application.Interfaces.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSI.Authentication.Application.UseCases.ChangeProfile
{
    public sealed class ChangeProfileHandler
    {
        private readonly IUserAccountRepository repo;
        public ChangeProfileHandler(IUserAccountRepository repo) => this.repo = repo;

        public async Task<ChangeProfileResponse> HandleAsync(ChangeProfileCommand cmd, CancellationToken ct)
        {
            var email = Email.Create(cmd.Email);
            var user = await repo.GetByEmailAsync(email, ct) ?? throw new NotFoundException("Usuário não encontrado");

            var old = user.Profile.Name.Value;
            var newProfileName = FSI.Authentication.Domain.ValueObjects.ProfileName.Create(cmd.NewProfile);
            var newProfile = new Profile(newProfileName);

            user.ChangeProfile(newProfile);
            await repo.UpdateAsync(user, ct);

            return new ChangeProfileResponse(user.Id, old, newProfileName.Value);
        }
    }
}

using System.Threading;
using System.Threading.Tasks;
using FSI.Authentication.Domain.Interfaces;

namespace FSI.Authentication.Application.UseCases.GetProfile
{
    public sealed class GetProfileHandler
    {
        private readonly IProfileService profileService;

        public GetProfileHandler(IProfileService profileService)
        {
            this.profileService = profileService;
        }

        public async Task<GetProfileOutput?> Handle(string email, CancellationToken ct = default)
        {
            var profile = await profileService.GetByEmailAsync(email, ct);
            if (profile is null) return null;
            return new GetProfileOutput(profile.Email, profile.ProfileName);
        }
    }
}

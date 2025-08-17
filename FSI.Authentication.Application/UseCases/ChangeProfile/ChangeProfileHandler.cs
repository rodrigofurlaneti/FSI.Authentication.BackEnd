using Result = FluentResults.Result;
using FluentResults;
using MediatR;
using FSI.Authentication.Domain.Interfaces;
using FSI.Authentication.Domain.Abstractions;
using FSI.Authentication.Application.UseCases.GetProfile;

namespace FSI.Authentication.Application.UseCases.ChangeProfile
{
    public sealed class ChangeProfileHandler
        : IRequestHandler<ChangeProfileCommand, Result<GetProfileOutput>>
    {
        private readonly IProfileService profileService;
        private readonly IClock clock;

        public ChangeProfileHandler(IProfileService profileService, IClock clock)
        {
            this.profileService = profileService;
            this.clock = clock;
        }

        public async Task<Result<GetProfileOutput>> Handle(
            ChangeProfileCommand request, CancellationToken cancellationToken)
        {
            var user = await profileService.GetByEmailAsync(request.Email, cancellationToken);
            if (user is null)
                return Result.Fail($"Profile not found for {request.Email}");

            // domínio expõe ChangeProfile
            user.ChangeProfile(request.NewProfileName, clock);

            // persiste (se a infra exigir, você pode implementar ChangeProfileAsync para fazer isso)
            var updated = await profileService.ChangeProfileAsync(request.Email, request.NewProfileName, cancellationToken);

            var output = new GetProfileOutput(updated.Email, updated.ProfileName);
            return Result.Ok(output);
        }
    }
}

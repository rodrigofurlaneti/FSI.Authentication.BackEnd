using MediatR;
using FluentResults;
using FSI.Authentication.Application.UseCases.GetProfile;

namespace FSI.Authentication.Application.UseCases.ChangeProfile
{
    public sealed record ChangeProfileCommand(
        string Email,
        string NewProfileName
    ) : IRequest<Result<GetProfileOutput>>;
}
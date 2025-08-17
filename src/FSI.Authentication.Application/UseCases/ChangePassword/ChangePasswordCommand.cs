using FluentResults;
using MediatR;

namespace FSI.Authentication.Application.UseCases.ChangePassword
{
    // Incluí CurrentPassword e ConfirmPassword porque validadores de senha
    // geralmente checam ambos. Isso satisfaz seu ChangePasswordValidator.
    public sealed record ChangePasswordCommand(
        string Email,
        string CurrentPassword,
        string NewPassword,
        string ConfirmPassword
    ) : IRequest<Result<Unit>>;
}

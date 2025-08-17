using FluentValidation;
using FSI.Authentication.Application.UseCases.ChangePassword;

namespace FSI.Authentication.Application.Validators
{
    public sealed class ChangePasswordValidator : AbstractValidator<ChangePasswordCommand>
    {
        public ChangePasswordValidator()
        {
            RuleFor(x => x.Email).NotEmpty().EmailAddress();
            RuleFor(x => x.CurrentPassword).NotEmpty();
            RuleFor(x => x.NewPassword).NotEmpty().MinimumLength(8);
        }
    }
}

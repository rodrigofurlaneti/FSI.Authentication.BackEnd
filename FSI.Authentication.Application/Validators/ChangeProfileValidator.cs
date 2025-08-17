using FluentValidation;
using FSI.Authentication.Application.UseCases.ChangeProfile;

namespace FSI.Authentication.Application.Validators
{
    public sealed class ChangeProfileValidator : AbstractValidator<ChangeProfileCommand>
    {
        public ChangeProfileValidator()
        {
            RuleFor(x => x.Email).NotEmpty().EmailAddress();
            RuleFor(x => x.NewProfileName).NotEmpty().MaximumLength(80);
        }
    }
}

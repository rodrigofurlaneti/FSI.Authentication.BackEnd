using FluentValidation;
using FSI.Authentication.Application.UseCases.Login;

namespace FSI.Authentication.Application.Validators
{
    public sealed class LoginValidator : AbstractValidator<LoginCommand>
    {
        public LoginValidator()
        {
            RuleFor(x => x.Email).NotEmpty().EmailAddress();
            RuleFor(x => x.Password).NotEmpty();
        }
    }
}

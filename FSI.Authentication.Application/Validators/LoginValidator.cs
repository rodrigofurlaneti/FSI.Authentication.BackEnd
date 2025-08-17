using FSI.Authentication.Application.UseCases.Login;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSI.Authentication.Application.Validators
{
    public sealed class LoginValidator : IValidator<LoginCommand>
    {
        public void ValidateAndThrow(LoginCommand c)
        {
            if (string.IsNullOrWhiteSpace(c.Email)) throw new ValidationAppException("E-mail obrigatório");
            if (string.IsNullOrWhiteSpace(c.Password)) throw new ValidationAppException("Senha obrigatória");
        }
    }
}

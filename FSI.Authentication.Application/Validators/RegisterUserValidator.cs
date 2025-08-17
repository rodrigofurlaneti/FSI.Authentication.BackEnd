using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSI.Authentication.Application.Validators
{
    public sealed class RegisterUserValidator : IValidator<RegisterUserCommand>
    {
        public void ValidateAndThrow(RegisterUserCommand c)
        {
            if (string.IsNullOrWhiteSpace(c.Email)) throw new ValidationAppException("E-mail obrigatório");
            if (string.IsNullOrWhiteSpace(c.FirstName)) throw new ValidationAppException("Nome obrigatório");
            if (string.IsNullOrWhiteSpace(c.Password)) throw new ValidationAppException("Senha obrigatória");
            if (string.IsNullOrWhiteSpace(c.Profile)) throw new ValidationAppException("Perfil obrigatório");
        }
    }
}

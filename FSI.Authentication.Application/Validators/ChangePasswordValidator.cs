using FSI.Authentication.Application.UseCases.ChangePassword;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSI.Authentication.Application.Validators
{
    public sealed class ChangePasswordValidator : IValidator<ChangePasswordCommand>
    {
        public void ValidateAndThrow(ChangePasswordCommand c)
        {
            if (string.IsNullOrWhiteSpace(c.Email)) throw new ValidationAppException("E-mail obrigatório");
            if (string.IsNullOrWhiteSpace(c.CurrentPassword)) throw new ValidationAppException("Senha atual obrigatória");
            if (string.IsNullOrWhiteSpace(c.NewPassword)) throw new ValidationAppException("Nova senha obrigatória");
        }
    }
}

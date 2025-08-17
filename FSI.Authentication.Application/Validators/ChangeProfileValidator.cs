using FSI.Authentication.Application.UseCases.ChangeProfile;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSI.Authentication.Application.Validators
{
    public sealed class ChangeProfileValidator : IValidator<ChangeProfileCommand>
    {
        public void ValidateAndThrow(ChangeProfileCommand c)
        {
            if (string.IsNullOrWhiteSpace(c.Email)) throw new ValidationAppException("E-mail obrigatório");
            if (string.IsNullOrWhiteSpace(c.NewProfile)) throw new ValidationAppException("Novo perfil obrigatório");
        }
    }
}

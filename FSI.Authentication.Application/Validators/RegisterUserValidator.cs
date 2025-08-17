using FSI.Authentication.Application.UseCases.RegisterUser;

namespace FSI.Authentication.Application.Validators
{
    // Validador “manual” só pra compilar sem FluentValidation
    public sealed class RegisterUserValidator
    {
        public void ValidateAndThrow(RegisterUserCommand cmd)
        {
            if (string.IsNullOrWhiteSpace(cmd.Email)) throw new ArgumentException("Email obrigatório.");
            if (!cmd.Email.Contains('@')) throw new ArgumentException("Email inválido.");
            if (string.IsNullOrWhiteSpace(cmd.FirstName)) throw new ArgumentException("FirstName obrigatório.");
            if (cmd.FirstName.Length > 100) throw new ArgumentException("FirstName muito longo.");
            if (cmd.LastName?.Length > 100) throw new ArgumentException("LastName muito longo.");
            if (string.IsNullOrWhiteSpace(cmd.ProfileName)) throw new ArgumentException("ProfileName obrigatório.");
            if (cmd.ProfileName.Length > 80) throw new ArgumentException("ProfileName muito longo.");
            if (string.IsNullOrWhiteSpace(cmd.Password) || cmd.Password.Length < 8)
                throw new ArgumentException("Senha mínima de 8 caracteres.");
        }
    }
}

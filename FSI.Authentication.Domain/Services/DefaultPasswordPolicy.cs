using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSI.Authentication.Domain.Services
{
    public sealed class DefaultPasswordPolicy : IPasswordPolicy
    {
        public bool IsStrong(string rawPassword, out string? reason)
        {
            reason = null;
            if (string.IsNullOrWhiteSpace(rawPassword)) { reason = "Senha vazia"; return false; }
            if (rawPassword.Length < 8) { reason = "Mínimo 8 caracteres"; return false; }
            if (!rawPassword.Any(char.IsUpper)) { reason = "Precisa de maiúscula"; return false; }
            if (!rawPassword.Any(char.IsLower)) { reason = "Precisa de minúscula"; return false; }
            if (!rawPassword.Any(char.IsDigit)) { reason = "Precisa de dígito"; return false; }
            return true;
        }
    }
}

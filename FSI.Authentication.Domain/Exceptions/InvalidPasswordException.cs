using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSI.Authentication.Domain.Exceptions
{
    public sealed class InvalidPasswordException : DomainException { public InvalidPasswordException(string reason) : base($"Senha inválida: {reason}") { } }
}

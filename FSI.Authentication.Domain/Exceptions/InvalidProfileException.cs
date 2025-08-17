using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSI.Authentication.Domain.Exceptions
{
    public sealed class InvalidProfileException : DomainException
    {
        public InvalidProfileException(string profile) : base($"Perfil inválido: {profile}. Valores aceitos: Gerente, Diretor, Supervisor, Coordenador, Analista, Estagiário") { }
    }
}

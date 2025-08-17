using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSI.Authentication.Domain.ValueObjects
{
    /// <summary>
    /// Nome do Perfil permitido. VO "fechado" garante um entre os válidos.
    /// </summary>
    public sealed record ProfileName
    {
        public string Value { get; }
        private ProfileName(string v) => Value = v;

        // Conjunto permitido
        public static readonly string Gerente = "Gerente";
        public static readonly string Diretor = "Diretor";
        public static readonly string Supervisor = "Supervisor";
        public static readonly string Coordenador = "Coordenador";
        public static readonly string Analista = "Analista";
        public static readonly string Estagiario = "Estagiário";

        private static readonly HashSet<string> Allowed = new(StringComparer.OrdinalIgnoreCase)
    {
        Gerente, Diretor, Supervisor, Coordenador, Analista, Estagiario
    };

        public static ProfileName Create(string value)
        {
            if (string.IsNullOrWhiteSpace(value)) throw new ArgumentException("Perfil obrigatório.");
            var normalized = Normalize(value);
            if (!Allowed.Contains(normalized))
                throw new Exceptions.InvalidProfileException(value);
            return new ProfileName(normalized);
        }

        public static string Normalize(string v)
        {
            v = v.Trim();
            // Normaliza capitalização
            return v switch
            {
                var s when s.Equals(Gerente, StringComparison.OrdinalIgnoreCase) => Gerente,
                var s when s.Equals(Diretor, StringComparison.OrdinalIgnoreCase) => Diretor,
                var s when s.Equals(Supervisor, StringComparison.OrdinalIgnoreCase) => Supervisor,
                var s when s.Equals(Coordenador, StringComparison.OrdinalIgnoreCase) => Coordenador,
                var s when s.Equals(Analista, StringComparison.OrdinalIgnoreCase) => Analista,
                var s when s.Equals(Estagiario, StringComparison.OrdinalIgnoreCase) || s.Equals("Estagiaria", StringComparison.OrdinalIgnoreCase) || s.Equals("Estagiário", StringComparison.OrdinalIgnoreCase) => Estagiario,
                _ => v
            };
        }

        public override string ToString() => Value;
    }
}

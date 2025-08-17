using System;
using System.Text.RegularExpressions;

namespace FSI.Authentication.Domain.ValueObjects
{
    // Pode ser record struct ou class. Vou usar class para simplicidade.
    public sealed class Email : IEquatable<Email>
    {
        private static readonly Regex Pattern =
            new(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.Compiled | RegexOptions.CultureInvariant);

        public string Value { get; }

        // Construtor público que aceita 1 argumento (resolve o erro “não contém um construtor que aceita 1 argumento”)
        public Email(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Email vazio.", nameof(value));

            var normalized = value.Trim();
            if (!Pattern.IsMatch(normalized))
                throw new ArgumentException("Email inválido.", nameof(value));

            Value = normalized;
        }

        // Conversões implícitas para reduzir fricção com DTOs/EF/infra
        public static implicit operator string(Email email) => email?.Value ?? string.Empty;
        public static implicit operator Email(string value) => new Email(value);

        public override string ToString() => Value;

        #region Equality
        public bool Equals(Email? other) => other is not null &&
                                            StringComparer.OrdinalIgnoreCase.Equals(Value, other.Value);

        public override bool Equals(object? obj) => obj is Email e && Equals(e);

        public override int GetHashCode() => StringComparer.OrdinalIgnoreCase.GetHashCode(Value);
        #endregion
    }
}

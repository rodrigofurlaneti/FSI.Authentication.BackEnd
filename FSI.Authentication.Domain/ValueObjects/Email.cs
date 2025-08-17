using System.Text.RegularExpressions;

namespace FSI.Authentication.Domain.ValueObjects
{
    public sealed record Email
    {
        private static readonly Regex Rx = new(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.Compiled);
        public string Value { get; }
        private Email(string v) => Value = v;
        public static Email Create(string v)
        {
            if (string.IsNullOrWhiteSpace(v) || !Rx.IsMatch(v))
                throw new ArgumentException("E-mail inválido.");
            return new Email(v.Trim().ToLowerInvariant());
        }
        public override string ToString() => Value;
    }
}

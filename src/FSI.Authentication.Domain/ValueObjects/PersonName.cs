using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSI.Authentication.Domain.ValueObjects
{
    public sealed record PersonName
    {
        public string FirstName { get; }
        public string? LastName { get; }
        private PersonName(string first, string? last) { FirstName = first; LastName = last; }
        public static PersonName Create(string first, string? last)
        {
            if (string.IsNullOrWhiteSpace(first)) throw new ArgumentException("Nome obrigatório.");
            return new PersonName(first.Trim(), string.IsNullOrWhiteSpace(last) ? null : last!.Trim());
        }
        public override string ToString() => string.IsNullOrWhiteSpace(LastName) ? FirstName : $"{FirstName} {LastName}";
    }
}

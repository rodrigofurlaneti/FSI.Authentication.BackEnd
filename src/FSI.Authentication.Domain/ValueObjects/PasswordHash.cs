using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSI.Authentication.Domain.ValueObjects
{
    public sealed record PasswordHash
    {
        public string Value { get; }
        private PasswordHash(string value) => Value = value;
        public static PasswordHash FromHashed(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Hash de senha não pode ser vazio.");
            return new PasswordHash(value);
        }
    }
}

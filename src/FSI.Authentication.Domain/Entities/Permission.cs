using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSI.Authentication.Domain.Entities
{
    public sealed class Permission : FSI.Authentication.Domain.Abstractions.Entity
    {
        public string Code { get; private set; } = default!; // ex: billing.read
        public string? Description { get; private set; }
        private Permission() { }
        public Permission(string code, string? description)
        {
            if (string.IsNullOrWhiteSpace(code)) throw new ArgumentException("Código de permissão é obrigatório.");
            Code = code.Trim().ToLowerInvariant();
            Description = string.IsNullOrWhiteSpace(description) ? null : description!.Trim();
        }
    }
}

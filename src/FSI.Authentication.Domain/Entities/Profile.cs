using FSI.Authentication.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSI.Authentication.Domain.Entities
{
    /// <summary>
    /// Perfil do usuário (ex.: Gerente, Diretor). Perfis podem carregar permissões.
    /// </summary>
    public sealed class Profile : FSI.Authentication.Domain.Abstractions.Entity
    {
        private readonly List<Permission> _permissions = new();
        public ProfileName Name { get; private set; } = default!;
        public ReadOnlyCollection<Permission> Permissions => _permissions.AsReadOnly();

        private Profile() { }
        public Profile(ProfileName name)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
        }

        public void AddPermission(Permission p)
        {
            if (p is null) throw new ArgumentNullException(nameof(p));
            if (_permissions.Any(x => x.Code == p.Code)) return;
            _permissions.Add(p);
        }
    }

}

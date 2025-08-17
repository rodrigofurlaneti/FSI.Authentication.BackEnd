using FSI.Authentication.Domain.Entities;
using FSI.Authentication.Domain.ValueObjects;

namespace FSI.Authentication.Domain.ProfilesCatalog
{
    public static class DefaultProfiles
    {
        public static Profile Gerente()
        {
            var p = new Profile(ProfileName.Create(ProfileName.Gerente));
            p.AddPermission(new Permission("dashboard.view", "Acessa dashboard"));
            p.AddPermission(new Permission("users.read", "Listar usuários"));
            p.AddPermission(new Permission("billing.read", "Ler faturas"));
            return p;
        }

        public static Profile Diretor()
        {
            var p = new Profile(ProfileName.Create(ProfileName.Diretor));
            p.AddPermission(new Permission("dashboard.view", "Acessa dashboard"));
            p.AddPermission(new Permission("users.read", "Listar usuários"));
            p.AddPermission(new Permission("users.write", "Gerenciar usuários"));
            p.AddPermission(new Permission("billing.approve", "Aprovar faturas"));
            return p;
        }

        public static Profile Supervisor()
        {
            var p = new Profile(ProfileName.Create(ProfileName.Supervisor));
            p.AddPermission(new Permission("users.read", null));
            p.AddPermission(new Permission("billing.read", null));
            return p;
        }

        public static Profile Coordenador()
        {
            var p = new Profile(ProfileName.Create(ProfileName.Coordenador));
            p.AddPermission(new Permission("billing.read", null));
            p.AddPermission(new Permission("tasks.assign", "Atribuir tarefas"));
            return p;
        }

        public static Profile Analista()
        {
            var p = new Profile(ProfileName.Create(ProfileName.Analista));
            p.AddPermission(new Permission("billing.read", null));
            return p;
        }

        public static Profile Estagiario()
        {
            var p = new Profile(ProfileName.Create(ProfileName.Estagiario));
            p.AddPermission(new Permission("docs.read", "Ler documentação"));
            return p;
        }
    }

}

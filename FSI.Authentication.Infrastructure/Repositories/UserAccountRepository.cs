using FSI.Authentication.Infrastructure.Persistence;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSI.Authentication.Infrastructure.Repositories
{
    /// <summary>
    /// Repositório baseado em procedures:
    /// - usp_User_GetByEmail @Email
    /// - usp_User_Insert     (...campos)
    /// - usp_User_Update     (...campos)
    /// Ajuste os nomes/colunas conforme seu banco.
    /// </summary>
    public sealed class UserAccountRepository : IUserAccountRepository
    {
        private readonly DbSession _session;
        public UserAccountRepository(DbSession session) => _session = session;

        public async Task<UserAccount?> GetByEmailAsync(Email email, CancellationToken ct)
        {
            using var cmd = new SqlCommand("dbo.usp_User_GetByEmail", _session.Connection, _session.Transaction)
            { CommandType = CommandType.StoredProcedure };

            cmd.Parameters.Add(new SqlParameter("@Email", SqlDbType.NVarChar, 256) { Value = email.Value });

            using var rdr = await cmd.ExecuteReaderAsync(ct);
            if (!await rdr.ReadAsync(ct)) return null;

            // Leitura do usuário
            var userId = rdr.GetGuid(rdr.GetOrdinal("UserId"));
            var firstName = rdr.GetString(rdr.GetOrdinal("FirstName"));
            var lastName = rdr.IsDBNull(rdr.GetOrdinal("LastName")) ? null : rdr.GetString(rdr.GetOrdinal("LastName"));
            var passwordHash = rdr.GetString(rdr.GetOrdinal("PasswordHash"));
            var isActive = rdr.GetBoolean(rdr.GetOrdinal("IsActive"));
            var failedCount = rdr.GetInt32(rdr.GetOrdinal("FailedLoginCount"));
            var lockoutEndUtc = rdr.IsDBNull(rdr.GetOrdinal("LockoutEndUtc")) ? (DateTime?)null : rdr.GetDateTime(rdr.GetOrdinal("LockoutEndUtc"));
            var profileName = rdr.GetString(rdr.GetOrdinal("ProfileName"));

            // Coletar permissões (pode vir 1 linha por permissão)
            var permissions = new List<Permission>();
            if (!rdr.IsDBNull(rdr.GetOrdinal("PermissionCode")))
            {
                var code = rdr.GetString(rdr.GetOrdinal("PermissionCode"));
                var desc = rdr.IsDBNull(rdr.GetOrdinal("PermissionDescription")) ? null : rdr.GetString(rdr.GetOrdinal("PermissionDescription"));
                permissions.Add(new Permission(code, desc));
            }
            while (await rdr.ReadAsync(ct))
            {
                if (!rdr.IsDBNull(rdr.GetOrdinal("PermissionCode")))
                {
                    var code = rdr.GetString(rdr.GetOrdinal("PermissionCode"));
                    var desc = rdr.IsDBNull(rdr.GetOrdinal("PermissionDescription")) ? null : rdr.GetString(rdr.GetOrdinal("PermissionDescription"));
                    permissions.Add(new Permission(code, desc));
                }
            }

            // Mapear para VO/Entidades de domínio
            var nameVo = PersonName.Create(firstName, lastName);
            var hashVo = PasswordHash.FromHashed(passwordHash);
            var profName = ProfileName.Create(profileName);
            var profile = new Profile(profName, permissions);

            // TODO: Ajuste para seu agregado.
            // Aqui assumimos que existe um "método de hidratação" no domínio.
            // Ex.: UserAccount.Hydrate(id, email, name, hash, profile, isActive, failedCount, lockoutEndUtc)
            var user = UserAccount.Hydrate(
                userId,
                email,
                nameVo,
                hashVo,
                profile,
                isActive,
                failedCount,
                lockoutEndUtc
            );

            return user;
        }

        public async Task AddAsync(UserAccount user, CancellationToken ct)
        {
            using var cmd = new SqlCommand("dbo.usp_User_Insert", _session.Connection, _session.Transaction)
            { CommandType = CommandType.StoredProcedure };

            cmd.Parameters.Add(new SqlParameter("@UserId", SqlDbType.UniqueIdentifier) { Value = user.Id });
            cmd.Parameters.Add(new SqlParameter("@Email", SqlDbType.NVarChar, 256) { Value = user.Email.Value });
            cmd.Parameters.Add(new SqlParameter("@FirstName", SqlDbType.NVarChar, 100) { Value = user.Name.FirstName });
            cmd.Parameters.Add(new SqlParameter("@LastName", SqlDbType.NVarChar, 100) { Value = (object?)user.Name.LastName ?? DBNull.Value });
            cmd.Parameters.Add(new SqlParameter("@PasswordHash", SqlDbType.NVarChar, 500) { Value = user.PasswordHash.Value });
            cmd.Parameters.Add(new SqlParameter("@IsActive", SqlDbType.Bit) { Value = user.IsActive });
            cmd.Parameters.Add(new SqlParameter("@ProfileName", SqlDbType.NVarChar, 80) { Value = user.Profile.Name.Value });

            await cmd.ExecuteNonQueryAsync(ct);

            // (Opcional) Persistir permissões de perfil se forem por usuário
            // Caso as permissões venham de um catálogo de perfil compartilhado, ignore.
        }

        public async Task UpdateAsync(UserAccount user, CancellationToken ct)
        {
            using var cmd = new SqlCommand("dbo.usp_User_Update", _session.Connection, _session.Transaction)
            { CommandType = CommandType.StoredProcedure };

            cmd.Parameters.Add(new SqlParameter("@UserId", SqlDbType.UniqueIdentifier) { Value = user.Id });
            cmd.Parameters.Add(new SqlParameter("@Email", SqlDbType.NVarChar, 256) { Value = user.Email.Value });
            cmd.Parameters.Add(new SqlParameter("@FirstName", SqlDbType.NVarChar, 100) { Value = user.Name.FirstName });
            cmd.Parameters.Add(new SqlParameter("@LastName", SqlDbType.NVarChar, 100) { Value = (object?)user.Name.LastName ?? DBNull.Value });
            cmd.Parameters.Add(new SqlParameter("@PasswordHash", SqlDbType.NVarChar, 500) { Value = user.PasswordHash.Value });
            cmd.Parameters.Add(new SqlParameter("@IsActive", SqlDbType.Bit) { Value = user.IsActive });
            cmd.Parameters.Add(new SqlParameter("@ProfileName", SqlDbType.NVarChar, 80) { Value = user.Profile.Name.Value });
            cmd.Parameters.Add(new SqlParameter("@FailedLoginCount", SqlDbType.Int) { Value = user.FailedLoginCount });
            cmd.Parameters.Add(new SqlParameter("@LockoutEndUtc", SqlDbType.DateTime2) { Value = (object?)user.LockoutEndUtc ?? DBNull.Value });

            await cmd.ExecuteNonQueryAsync(ct);
        }
    }
}

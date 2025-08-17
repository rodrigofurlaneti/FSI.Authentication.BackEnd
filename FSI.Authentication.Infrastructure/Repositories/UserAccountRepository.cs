using FSI.Authentication.Domain.Aggregates;   // UserAccount
using FSI.Authentication.Domain.Interfaces;   // IUserAccountRepository  (novo using)
using FSI.Authentication.Infrastructure.Persistence;
using Microsoft.Data.SqlClient;
using System.Data;

namespace FSI.Authentication.Infrastructure.Repositories
{
    /// <summary>
    /// Repositório baseado em procedures:
    /// - usp_User_GetByEmail @Email
    /// - usp_User_GetById    @UserId
    /// - usp_User_Insert     (...campos)
    /// - usp_User_Update     (...campos)
    /// Ajuste os nomes/colunas conforme seu banco.
    /// </summary>
    public sealed class UserAccountRepository : IUserAccountRepository
    {
        private readonly DbSession _session;
        public UserAccountRepository(DbSession session) => _session = session;

        public async Task<UserAccount?> GetByEmailAsync(string email, CancellationToken ct = default)
        {
            using var cmd = new SqlCommand("dbo.usp_User_GetByEmail", _session.Connection, _session.Transaction)
            { CommandType = CommandType.StoredProcedure };

            cmd.Parameters.Add(new SqlParameter("@Email", SqlDbType.NVarChar, 256) { Value = email });

            using var rdr = await cmd.ExecuteReaderAsync(ct);
            if (!await rdr.ReadAsync(ct)) return null;

            var user = MapReaderToUser(rdr);
            return user;
        }

        public async Task<UserAccount?> GetByIdAsync(Guid id, CancellationToken ct = default)
        {
            using var cmd = new SqlCommand("dbo.usp_User_GetById", _session.Connection, _session.Transaction)
            { CommandType = CommandType.StoredProcedure };

            cmd.Parameters.Add(new SqlParameter("@UserId", SqlDbType.UniqueIdentifier) { Value = id });

            using var rdr = await cmd.ExecuteReaderAsync(ct);
            if (!await rdr.ReadAsync(ct)) return null;

            var user = MapReaderToUser(rdr);
            return user;
        }

        public async Task AddAsync(UserAccount user, CancellationToken ct = default)
        {
            using var cmd = new SqlCommand("dbo.usp_User_Insert", _session.Connection, _session.Transaction)
            { CommandType = CommandType.StoredProcedure };

            cmd.Parameters.Add(new SqlParameter("@UserId", SqlDbType.UniqueIdentifier) { Value = user.UserId });
            cmd.Parameters.Add(new SqlParameter("@Email", SqlDbType.NVarChar, 256) { Value = user.Email });
            cmd.Parameters.Add(new SqlParameter("@FirstName", SqlDbType.NVarChar, 100) { Value = user.FirstName });
            cmd.Parameters.Add(new SqlParameter("@LastName", SqlDbType.NVarChar, 100) { Value = (object?)user.LastName ?? DBNull.Value });
            cmd.Parameters.Add(new SqlParameter("@PasswordHash", SqlDbType.NVarChar, 500) { Value = user.PasswordHash });
            cmd.Parameters.Add(new SqlParameter("@IsActive", SqlDbType.Bit) { Value = user.IsActive });
            cmd.Parameters.Add(new SqlParameter("@FailedLoginCount", SqlDbType.Int) { Value = user.FailedLoginCount });
            cmd.Parameters.Add(new SqlParameter("@LockoutEndUtc", SqlDbType.DateTime2) { Value = (object?)user.LockoutEndUtc?.UtcDateTime ?? DBNull.Value });
            cmd.Parameters.Add(new SqlParameter("@ProfileName", SqlDbType.NVarChar, 80) { Value = user.ProfileName });

            await cmd.ExecuteNonQueryAsync(ct);
        }

        public async Task UpdateAsync(UserAccount user, CancellationToken ct = default)
        {
            using var cmd = new SqlCommand("dbo.usp_User_Update", _session.Connection, _session.Transaction)
            { CommandType = CommandType.StoredProcedure };

            cmd.Parameters.Add(new SqlParameter("@UserId", SqlDbType.UniqueIdentifier) { Value = user.UserId });
            cmd.Parameters.Add(new SqlParameter("@Email", SqlDbType.NVarChar, 256) { Value = user.Email });
            cmd.Parameters.Add(new SqlParameter("@FirstName", SqlDbType.NVarChar, 100) { Value = user.FirstName });
            cmd.Parameters.Add(new SqlParameter("@LastName", SqlDbType.NVarChar, 100) { Value = (object?)user.LastName ?? DBNull.Value });
            cmd.Parameters.Add(new SqlParameter("@PasswordHash", SqlDbType.NVarChar, 500) { Value = user.PasswordHash });
            cmd.Parameters.Add(new SqlParameter("@IsActive", SqlDbType.Bit) { Value = user.IsActive });
            cmd.Parameters.Add(new SqlParameter("@FailedLoginCount", SqlDbType.Int) { Value = user.FailedLoginCount });
            cmd.Parameters.Add(new SqlParameter("@LockoutEndUtc", SqlDbType.DateTime2) { Value = (object?)user.LockoutEndUtc?.UtcDateTime ?? DBNull.Value });
            cmd.Parameters.Add(new SqlParameter("@ProfileName", SqlDbType.NVarChar, 80) { Value = user.ProfileName });

            await cmd.ExecuteNonQueryAsync(ct);
        }

        public Task SaveChangesAsync(CancellationToken ct = default)
        {
            // Se você usar transação externa no DbSession, faça commit aqui.
            // Como não temos contexto do seu DbSession, deixo no-op.
            return Task.CompletedTask;
        }

        // ---------- Helpers ----------
        private static UserAccount MapReaderToUser(SqlDataReader rdr)
        {
            var userId = rdr.GetGuid(rdr.GetOrdinal("UserId"));
            var email = rdr.GetString(rdr.GetOrdinal("Email"));
            var firstName = rdr.GetString(rdr.GetOrdinal("FirstName"));
            var lastName = rdr.IsDBNull(rdr.GetOrdinal("LastName")) ? null : rdr.GetString(rdr.GetOrdinal("LastName"));
            var passwordHash = rdr.GetString(rdr.GetOrdinal("PasswordHash"));
            var isActive = rdr.GetBoolean(rdr.GetOrdinal("IsActive"));
            var failedCount = rdr.GetInt32(rdr.GetOrdinal("FailedLoginCount"));

            DateTimeOffset? lockoutEnd = null;
            var ordLock = rdr.GetOrdinal("LockoutEndUtc");
            if (ordLock >= 0 && !rdr.IsDBNull(ordLock))
            {
                // Assume que o DateTime no banco está em UTC
                var dt = rdr.GetDateTime(ordLock);
                lockoutEnd = DateTime.SpecifyKind(dt, DateTimeKind.Utc);
            }

            var profileName = rdr.GetString(rdr.GetOrdinal("ProfileName"));

            // Usa o construtor do seu domínio atual
            var user = new UserAccount(
                userId,
                email,
                firstName,
                lastName,
                passwordHash,
                profileName,
                isActive,
                failedCount,
                lockoutEnd
            );

            return user;
        }
    }
}

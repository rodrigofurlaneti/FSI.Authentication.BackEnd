using FSI.Authentication.Domain.Aggregates;
using FSI.Authentication.Domain.ValueObjects;           
using FSI.Authentication.Infrastructure.Persistence;
using Microsoft.Data.SqlClient;
using System.Data;
using AppRepos = FSI.Authentication.Application.Interfaces.Repositories;

namespace FSI.Authentication.Infrastructure.Repositories
{
    public sealed class UserAccountRepository : AppRepos.IUserAccountRepository
    {
        private readonly DbSession _session;
        public UserAccountRepository(DbSession session) => _session = session;

        public async Task<UserAccount?> GetByEmailAsync(Email email, CancellationToken ct = default) // << assinatura igual à interface
        {
            using var cmd = new SqlCommand("dbo.usp_User_GetByEmail", _session.Connection, _session.Transaction)
            { CommandType = CommandType.StoredProcedure };

            cmd.Parameters.Add(new SqlParameter("@Email", SqlDbType.NVarChar, 256) { Value = email.Value }); // << usa o VO

            using var rdr = await cmd.ExecuteReaderAsync(ct);
            if (!await rdr.ReadAsync(ct)) return null;

            return MapReaderToUser(rdr);
        }

        public async Task<UserAccount?> GetByIdAsync(Guid id, CancellationToken ct = default)
        {
            using var cmd = new SqlCommand("dbo.usp_User_GetById", _session.Connection, _session.Transaction)
            { CommandType = CommandType.StoredProcedure };

            cmd.Parameters.Add(new SqlParameter("@UserId", SqlDbType.UniqueIdentifier) { Value = id });

            using var rdr = await cmd.ExecuteReaderAsync(ct);
            if (!await rdr.ReadAsync(ct)) return null;

            return MapReaderToUser(rdr);
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

        public Task SaveChangesAsync(CancellationToken ct = default) => Task.CompletedTask;

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
                var dt = rdr.GetDateTime(ordLock);
                lockoutEnd = DateTime.SpecifyKind(dt, DateTimeKind.Utc);
            }

            var profileName = rdr.GetString(rdr.GetOrdinal("ProfileName"));

            return new UserAccount(
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
        }
    }
}

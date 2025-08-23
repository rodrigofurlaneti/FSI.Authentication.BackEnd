using FSI.Authentication.Application.Interfaces.Repositories;
using FSI.Authentication.Infrastructure.Persistence;
using System.Data;
using System.Data.Common;

namespace FSI.Authentication.Infrastructure.Repositories
{
    public sealed class AdminRepository : IAdminRepository
    {
        private readonly DbSession _dbSession;
        public AdminRepository(DbSession dbSession) => _dbSession = dbSession;

        public async Task<bool> TestDatabaseAsync(CancellationToken cancellationToken)
        {
            try
            {
                var conn = _dbSession.Connection;

                if (conn is DbConnection dbConn)
                {
                    if (dbConn.State != ConnectionState.Open)
                        await dbConn.OpenAsync(cancellationToken);

                    await using var cmd = dbConn.CreateCommand();
                    cmd.CommandText = "SELECT 1";
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandTimeout = 2; // curto pro health-check

                    if (_dbSession.Transaction is DbTransaction dbTx)
                        cmd.Transaction = dbTx;

                    var result = await cmd.ExecuteScalarAsync(cancellationToken);

                    return Convert.ToInt32(result) == 1;
                }
                else
                {
                    // Fallback: IDbConnection sem async
                    if (conn.State != ConnectionState.Open)
                        conn.Open();

                    using var cmd = conn.CreateCommand();
                    cmd.CommandText = "SELECT 1";
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandTimeout = 2;

                    if (_dbSession.Transaction != null)
                        cmd.Transaction = _dbSession.Transaction;

                    var result = cmd.ExecuteScalar();
                    return Convert.ToInt32(result) == 1;
                }
            }
            catch
            {
                return false;
            }
        }
    }
}

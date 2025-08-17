using System.Data;
using Microsoft.Data.SqlClient;

namespace FSI.Authentication.Infrastructure.Persistence
{
    public sealed class DbSession : IDisposable
    {
        public SqlConnection Connection { get; }
        public SqlTransaction? Transaction { get; private set; }

        public DbSession(ISqlConnectionFactory factory)
        {
            Connection = factory.CreateOpenConnection();
        }

        public async Task BeginAsync(CancellationToken ct)
        {
            if (Transaction is null)
                Transaction = (SqlTransaction)await Connection.BeginTransactionAsync(ct);
        }

        public Task CommitAsync(CancellationToken ct)
            => Transaction?.CommitAsync(ct) ?? Task.CompletedTask;

        public Task RollbackAsync(CancellationToken ct)
            => Transaction?.RollbackAsync(ct) ?? Task.CompletedTask;

        public void Dispose()
        {
            Transaction?.Dispose();
            Connection.Dispose();
        }
    }
}

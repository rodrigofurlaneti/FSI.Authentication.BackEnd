namespace FSI.Authentication.Infrastructure.Persistence;

using Microsoft.Data.SqlClient;
using System.Data;

public sealed class DbSession : IAsyncDisposable
{
    public SqlConnection Connection { get; }
    public SqlTransaction? Transaction { get; internal set; }

    public DbSession(ISqlConnectionFactory factory)
    {
        Connection = factory.CreateOpenConnection(); 
    }

    public async ValueTask DisposeAsync()
    {
        if (Transaction is not null)
        {
            try { await Transaction.RollbackAsync(); } catch { }
            await Transaction.DisposeAsync();
            Transaction = null;
        }

        if (Connection.State != ConnectionState.Closed)
            await Connection.CloseAsync();

        await Connection.DisposeAsync();
    }
}

namespace FSI.Authentication.Infrastructure.Persistence;

using FSI.Authentication.Application.Interfaces;
using Microsoft.Data.SqlClient;
using System.Data;

public sealed class UnitOfWork : IUnitOfWork
{
    private readonly DbSession _session;
    public UnitOfWork(DbSession session) => _session = session;

    public async Task<IUnitOfWorkTransaction> BeginAsync(CancellationToken ct = default)
    {
        if (_session.Connection.State != ConnectionState.Open)
            await _session.Connection.OpenAsync(ct);

        if (_session.Transaction is not null)
            throw new InvalidOperationException("Já existe uma transação ativa.");

        var tx = (SqlTransaction)await _session.Connection.BeginTransactionAsync(ct);
        _session.Transaction = tx;
        return new UowTx(_session, tx);
    }

    private sealed class UowTx : IUnitOfWorkTransaction
    {
        private readonly DbSession _s; private SqlTransaction? _tx;
        public UowTx(DbSession s, SqlTransaction tx) { _s = s; _tx = tx; }

        public async Task CommitAsync(CancellationToken ct = default)
        {
            if (_tx is null) return;
            await _tx.CommitAsync(ct);
            await _tx.DisposeAsync();
            _tx = null;
            _s.Transaction = null; // limpa imediatamente
        }

        public async Task RollbackAsync(CancellationToken ct = default)
        {
            if (_tx is null) return;
            try { await _tx.RollbackAsync(ct); } catch { }
            await _tx.DisposeAsync();
            _tx = null;
            _s.Transaction = null;
        }

        public async ValueTask DisposeAsync()
        {
            if (_tx is not null)
            {
                try { await _tx.RollbackAsync(); } catch { }
                await _tx.DisposeAsync();
                _tx = null;
            }
            _s.Transaction = null;
        }
    }
}

using FSI.Authentication.Application.Interfaces;   

namespace FSI.Authentication.Infrastructure.Persistence
{
    public sealed class UnitOfWork : IUnitOfWork
    {
        private readonly DbSession _session;
        public UnitOfWork(DbSession session) => _session = session;

        public async Task<IUnitOfWorkScope> BeginAsync(CancellationToken ct = default)
        {
            await _session.BeginAsync(ct);
            return new Scope(_session, ct);
        }

        private sealed class Scope : IUnitOfWorkScope
        {
            private readonly DbSession _s;
            private readonly CancellationToken _ct;
            private bool _completed;

            public Scope(DbSession s, CancellationToken ct) { _s = s; _ct = ct; }

            public async Task CommitAsync()
            {
                await _s.CommitAsync(_ct);
                _completed = true;
            }

            public void Dispose()
            {
                if (!_completed)
                    _s.RollbackAsync(_ct).GetAwaiter().GetResult();
            }

            public async ValueTask DisposeAsync()
            {
                if (!_completed)
                    await _s.RollbackAsync(_ct);
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSI.Authentication.Infrastructure.Persistence
{
    public sealed class UnitOfWork : IUnitOfWork
    {
        private readonly DbSession _session;
        public UnitOfWork(DbSession session) => _session = session;

        public async Task<IDisposable> BeginAsync(CancellationToken ct)
        {
            await _session.BeginAsync(ct);
            return new Scope(_session, ct);
        }

        private sealed class Scope : IDisposable
        {
            private readonly DbSession _s;
            private readonly CancellationToken _ct;
            private bool _completed;

            public Scope(DbSession s, CancellationToken ct) { _s = s; _ct = ct; }

            public void Dispose()
            {
                if (!_completed)
                {
                    // best-effort rollback
                    _s.RollbackAsync(_ct).GetAwaiter().GetResult();
                }
            }

            public async Task CommitAsync() { await _s.CommitAsync(_ct); _completed = true; }
        }
    }
}

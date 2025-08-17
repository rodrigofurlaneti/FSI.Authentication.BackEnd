using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSI.Authentication.Infrastructure.Persistence
{
    public interface IUnitOfWork
    {
        Task<IDisposable> BeginAsync(CancellationToken ct);
    }
}

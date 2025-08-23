using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSI.Authentication.Application.Interfaces
{
    public interface IUnitOfWorkScope : IDisposable, IAsyncDisposable
    {
        Task CommitAsync();
    }
}

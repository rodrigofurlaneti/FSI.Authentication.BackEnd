using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSI.Authentication.Application.Handlers
{

    public sealed class TransactionBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    {
        private readonly Func<CancellationToken, Task<IDisposable>> beginTx;
        public TransactionBehavior(Func<CancellationToken, Task<IDisposable>> beginTx) => this.beginTx = beginTx;

        public async Task<TResponse> HandleAsync(TRequest request, CancellationToken ct, Func<Task<TResponse>> next)
        {
            using var tx = await beginTx(ct);
            return await next();
        }
    }
}

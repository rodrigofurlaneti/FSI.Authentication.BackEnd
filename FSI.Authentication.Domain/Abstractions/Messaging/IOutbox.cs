using System.Threading;
using System.Threading.Tasks;

namespace FSI.Authentication.Domain.Abstractions.Messaging
{
    public interface IOutbox
    {
        Task EnqueueAsync(object message, CancellationToken ct = default);
    }
}

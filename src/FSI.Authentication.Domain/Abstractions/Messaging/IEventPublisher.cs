// Domain ➜ FSI.Authentication.Domain.Abstractions.Messaging
using System.Threading;
using System.Threading.Tasks;

namespace FSI.Authentication.Domain.Abstractions.Messaging
{
    public interface IEventPublisher
    {
        Task PublishAsync<T>(T notification, CancellationToken ct) where T : class;
    }
}


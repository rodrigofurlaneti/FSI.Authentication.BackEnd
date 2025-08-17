using System.Threading;
using System.Threading.Tasks;

namespace FSI.Authentication.Domain.Abstractions.Messaging
{
    public interface IEventPublisher
    {
        Task PublishAsync(object @event, CancellationToken ct = default);
    }
}

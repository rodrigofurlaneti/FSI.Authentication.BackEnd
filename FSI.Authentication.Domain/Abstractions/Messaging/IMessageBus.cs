using System.Threading;
using System.Threading.Tasks;

namespace FSI.Authentication.Domain.Abstractions.Messaging
{
    public interface IMessageBus
    {
        Task PublishAsync(object message, CancellationToken ct = default);
    }
}

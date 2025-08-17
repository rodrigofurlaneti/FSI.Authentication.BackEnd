using System.Threading;
using System.Threading.Tasks;

namespace FSI.Authentication.Application.Interfaces.Messaging
{
    public interface IMessageBus
    {
        public Task PublishAsync(object message, CancellationToken ct) => Task.CompletedTask;
        public Task PublishAsync<T>(T message, CancellationToken ct) => PublishAsync((object)message!, ct);

    }
}


using System.Threading;
using System.Threading.Tasks;
using FSI.Authentication.Domain.Abstractions.Messaging; // IOutbox, IMessageBus, IEventPublisher

namespace FSI.Authentication.Infrastructure.Outbox
{
    public sealed class SqlMessageBus : IMessageBus
    {
        public Task PublishAsync<T>(T message, CancellationToken ct) where T : class
            => PublishAsync((object)message!, ct);

        public Task PublishAsync(object message, CancellationToken ct)
        {
            return Task.CompletedTask;
        }
    }
}

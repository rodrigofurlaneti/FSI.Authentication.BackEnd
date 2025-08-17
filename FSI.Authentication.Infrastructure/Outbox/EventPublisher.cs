using System.Threading;
using System.Threading.Tasks;
using FSI.Authentication.Domain.Abstractions.Messaging;

namespace FSI.Authentication.Infrastructure.Outbox
{
    public sealed class EventPublisher : IEventPublisher
    {
        private readonly IOutbox _outbox;
        public EventPublisher(IOutbox outbox) => _outbox = outbox;

        public Task PublishAsync<T>(T notification, CancellationToken ct) where T : class
            => _outbox.EnqueueAsync(notification, ct);
    }
}

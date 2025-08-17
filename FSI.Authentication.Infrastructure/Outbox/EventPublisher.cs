using System.Threading;
using System.Threading.Tasks;
using FSI.Authentication.Domain.Abstractions.Messaging;

namespace FSI.Authentication.Infrastructure.Outbox
{
    /// <summary>
    /// Publisher que apenas registra no Outbox. O Worker/Consumer entrega de fato.
    /// </summary>
    public sealed class EventPublisher : IEventPublisher
    {
        private readonly IOutbox _outbox;
        public EventPublisher(IOutbox outbox) => _outbox = outbox;

        // Atalho genérico (opcional)
        public Task PublishAsync<T>(T notification, CancellationToken ct) where T : class
            => PublishAsync((object)notification!, ct);

        // Implementação que satisfaz a interface do Domain
        public Task PublishAsync(object @event, CancellationToken ct)
            => _outbox.EnqueueAsync(@event, ct);
    }
}

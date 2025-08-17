using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSI.Authentication.Infrastructure.Outbox
{
    /// <summary>
    /// Publisher que apenas registra no Outbox. O Worker/Consumer entrega de fato.
    /// </summary>
    public sealed class EventPublisher : IEventPublisher
    {
        private readonly IOutbox _outbox;
        public EventPublisher(IOutbox outbox) => _outbox = outbox;

        public Task PublishAsync<T>(T notification, CancellationToken ct) where T : class
            => _outbox.EnqueueAsync(notification, ct);
    }
}

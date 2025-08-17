using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSI.Authentication.Infrastructure.Outbox
{
    /// <summary>
    /// Exemplo de MessageBus. Ajuste para seu broker real (RabbitMQ, Kafka, etc.).
    /// Aqui não faz nada além de completar a interface.
    /// </summary>
    public sealed class SqlMessageBus : IMessageBus
    {
        public Task PublishAsync<T>(T message, CancellationToken ct) where T : class
            => Task.CompletedTask;
    }
}

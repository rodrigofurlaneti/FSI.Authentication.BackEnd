using System;
using System.Data;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;

using FSI.Authentication.Domain.Abstractions.Messaging;
using FSI.Authentication.Infrastructure.Outbox;
using FSI.Authentication.Infrastructure.Persistence;

namespace FSI.Authentication.Infrastructure
{
    public sealed class SqlOutbox : IOutbox
    {
        private readonly DbSession _session;
        public SqlOutbox(DbSession session) => _session = session;

        public Task EnqueueAsync<T>(T message, CancellationToken ct) where T : class
            => EnqueueAsync((object)message!, ct);

        public async Task EnqueueAsync(object message, CancellationToken ct)
        {
            var messageType = message.GetType();

            var outbox = new OutboxMessage
            {
                Id = Guid.NewGuid(),
                Type = messageType.FullName!,
                Payload = JsonSerializer.Serialize(message, messageType),
                OccurredOnUtc = DateTime.UtcNow
            };

            using var cmd = new SqlCommand("dbo.usp_Outbox_Insert", _session.Connection, _session.Transaction)
            { CommandType = CommandType.StoredProcedure };

            cmd.Parameters.Add(new SqlParameter("@Id", SqlDbType.UniqueIdentifier) { Value = outbox.Id });
            cmd.Parameters.Add(new SqlParameter("@Type", SqlDbType.NVarChar, 400) { Value = outbox.Type });
            cmd.Parameters.Add(new SqlParameter("@Payload", SqlDbType.NVarChar, -1) { Value = outbox.Payload });
            cmd.Parameters.Add(new SqlParameter("@OccurredOnUtc", SqlDbType.DateTime2) { Value = outbox.OccurredOnUtc });

            await cmd.ExecuteNonQueryAsync(ct);
        }
    }
}

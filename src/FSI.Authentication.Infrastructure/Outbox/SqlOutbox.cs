using System.Data;
using System.Text.Json;
using Microsoft.Data.SqlClient;
using FSI.Authentication.Infrastructure.Persistence;
using DomMsg = FSI.Authentication.Domain.Abstractions.Messaging;

namespace FSI.Authentication.Infrastructure.Outbox
{
    public sealed class SqlOutbox : DomMsg.IOutbox
    {
        private readonly DbSession _session;
        public SqlOutbox(DbSession session) => _session = session;

        // método não genérico da interface Domain
        public async Task EnqueueAsync(object message, CancellationToken ct)
        {
            var outbox = new OutboxMessage
            {
                Type = message.GetType().FullName!,
                Payload = JsonSerializer.Serialize(message),
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

        public Task EnqueueAsync<T>(T message, CancellationToken ct) where T : class
        {
            // serialize e salvar na tabela Outbox...
            return Task.CompletedTask;
        }
    }
}

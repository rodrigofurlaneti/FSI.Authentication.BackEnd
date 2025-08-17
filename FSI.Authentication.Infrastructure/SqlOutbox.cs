using FSI.Authentication.Infrastructure.Outbox;
using FSI.Authentication.Infrastructure.Persistence;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace FSI.Authentication.Infrastructure
{
    public sealed class SqlOutbox : IOutbox
    {
        private readonly DbSession _session;
        public SqlOutbox(DbSession session) => _session = session;

        public async Task EnqueueAsync<T>(T message, CancellationToken ct) where T : class
        {
            var outbox = new OutboxMessage
            {
                Type = typeof(T).FullName!,
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
    }
}

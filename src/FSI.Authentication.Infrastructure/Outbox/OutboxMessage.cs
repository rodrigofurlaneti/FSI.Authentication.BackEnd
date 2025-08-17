using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSI.Authentication.Infrastructure.Outbox
{
    public sealed class OutboxMessage
    {
        public Guid Id { get; init; } = Guid.NewGuid();
        public string Type { get; init; } = default!;
        public string Payload { get; init; } = default!;
        public DateTime OccurredOnUtc { get; init; } = DateTime.UtcNow;
        public DateTime? ProcessedOnUtc { get; set; }
        public int Attempts { get; set; }
    }
}

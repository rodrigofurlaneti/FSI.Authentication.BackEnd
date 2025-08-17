using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSI.Authentication.Domain.Abstractions
{
    public interface IClock
    {
        DateTimeOffset UtcNow { get; }
    }

    // Implementação padrão para uso em runtime/DI
    public sealed class SystemClock : IClock
    {
        public DateTimeOffset UtcNow => DateTimeOffset.UtcNow;
    }
}

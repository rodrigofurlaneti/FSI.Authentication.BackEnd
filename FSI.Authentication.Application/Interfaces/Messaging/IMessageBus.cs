using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSI.Authentication.Application.Interfaces.Messaging
{
    Task PublishAsync<T>(T message, CancellationToken ct) where T : class;
}

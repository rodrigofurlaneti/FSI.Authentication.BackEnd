using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSI.Authentication.Application.Interfaces.Services
{
    public interface IEventPublisher
    {
        Task PublishAsync<T>(T notification, CancellationToken ct) where T : class;
    }
}

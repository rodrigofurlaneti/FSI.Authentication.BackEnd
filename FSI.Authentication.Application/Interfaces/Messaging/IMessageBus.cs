using System.Threading;
using System.Threading.Tasks;

namespace FSI.Authentication.Application.Interfaces.Messaging
{
    public interface IMessageBus
    {
        Task PublishAsync<T>(T message, CancellationToken ct = default);
    }
}


using AppMess = FSI.Authentication.Application.Interfaces.Messaging;

public sealed class SqlMessageBus : AppMess.IMessageBus
{
    public Task PublishAsync<T>(T message, CancellationToken ct)
        => Task.CompletedTask; // implemente seu broker real aqui
}

namespace FSI.Authentication.Application.Interfaces.Services
{
    public interface IOutbox
    {
        Task EnqueueAsync<T>(T message, CancellationToken ct) where T : class;
    }
}

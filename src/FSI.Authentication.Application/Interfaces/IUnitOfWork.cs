namespace FSI.Authentication.Application.Interfaces
{
    public interface IUnitOfWork
    {
        Task<IUnitOfWorkTransaction> BeginAsync(CancellationToken ct = default);
    }
}

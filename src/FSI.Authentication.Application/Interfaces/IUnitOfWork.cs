namespace FSI.Authentication.Application.Interfaces
{
    public interface IUnitOfWork
    {
        Task<IUnitOfWorkScope> BeginAsync(CancellationToken ct = default);
    }
}

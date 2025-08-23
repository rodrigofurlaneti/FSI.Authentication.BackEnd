namespace FSI.Authentication.Application.Interfaces.Repositories
{
    public interface IAdminRepository
    {
        Task<bool> TestDatabaseAsync(CancellationToken cancellationToken);
    }
}

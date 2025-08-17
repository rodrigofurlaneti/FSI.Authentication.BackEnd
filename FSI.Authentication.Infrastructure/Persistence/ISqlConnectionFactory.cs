using Microsoft.Data.SqlClient;

namespace FSI.Authentication.Infrastructure.Persistence
{
    public interface ISqlConnectionFactory
    {
        SqlConnection CreateOpenConnection();
    }
}

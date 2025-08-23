namespace FSI.Authentication.Infrastructure.Persistence;

using Microsoft.Data.SqlClient;
using System.Data;

public static class SqlGuard
{
    public static async Task EnsureOpenAsync(SqlConnection conn, CancellationToken ct)
    {
        if (conn.State == ConnectionState.Open) return;
        try
        {
            await conn.OpenAsync(ct);
            using var cmd = new SqlCommand("SELECT 1", conn);
            await cmd.ExecuteScalarAsync(ct);
        }
        catch (SqlException ex)
        {
            throw new DataAccessException(DataAccessError.ConnectionOpenFailed,
                "Falha ao abrir/validar conexão com o banco de dados.", ex, ex.Number);
        }
    }

    public static async Task EnsureStoredProcExistsAsync(
        SqlConnection conn, string procFullName, SqlTransaction? tx, CancellationToken ct)
    {
        var schema = "dbo"; var name = procFullName;
        var dot = procFullName.IndexOf('.');
        if (dot > 0) { schema = procFullName[..dot]; name = procFullName[(dot + 1)..]; }

        const string sql = """
            SELECT 1
            FROM sys.procedures p
            JOIN sys.schemas s ON s.schema_id = p.schema_id
            WHERE s.name = @schema AND p.name = @name
            """;

        using var cmd = new SqlCommand(sql, conn, tx);
        cmd.Parameters.AddWithValue("@schema", schema);
        cmd.Parameters.AddWithValue("@name", name);
        var exists = await cmd.ExecuteScalarAsync(ct);

        if (exists is null)
            throw new DataAccessException(DataAccessError.StoredProcedureNotFound,
                $"Stored procedure '{schema}.{name}' não existe.", context: $"{schema}.{name}");
    }

    public static Exception Wrap(SqlException ex, string? context = null) => ex.Number switch
    {
        -2 => new DataAccessException(DataAccessError.SqlTimeout, "Timeout na operação no SQL Server.", ex, ex.Number, context),
        1205 => new DataAccessException(DataAccessError.DeadlockVictim, "Deadlock no SQL Server.", ex, ex.Number, context),
        2627 => new DataAccessException(DataAccessError.UniqueViolation, "Violação de unicidade.", ex, ex.Number, context),
        2601 => new DataAccessException(DataAccessError.UniqueViolation, "Violação de índice único.", ex, ex.Number, context),
        547 => new DataAccessException(DataAccessError.ForeignKeyViolation, "Violação de chave estrangeira.", ex, ex.Number, context),
        8152 => new DataAccessException(DataAccessError.StringTruncation, "Texto excede limite da coluna.", ex, ex.Number, context),
        18456 => new DataAccessException(DataAccessError.LoginFailed, "Falha de login no SQL Server.", ex, ex.Number, context),
        4060 => new DataAccessException(DataAccessError.DatabaseNotFound, "Banco inválido/inexistente.", ex, ex.Number, context),
        208 => new DataAccessException(DataAccessError.InvalidObject, "Objeto inválido/inexistente.", ex, ex.Number, context),
        2812 => new DataAccessException(DataAccessError.StoredProcedureNotFound, "Stored procedure não encontrada.", ex, ex.Number, context),
        _ => new DataAccessException(DataAccessError.Unknown, $"Erro SQL ({ex.Number}).", ex, ex.Number, context)
    };
}

using FSI.Authentication.Infrastructure.Persistence;

public sealed class DataAccessException : Exception
{
    public DataAccessError Reason { get; }
    public int? SqlErrorNumber { get; }
    public string? Context { get; }

    public DataAccessException(
        DataAccessError reason,
        string message,
        Exception? inner = null,
        int? sqlErrorNumber = null,
        string? context = null
    ) : base(message, inner)
    {
        Reason = reason;
        SqlErrorNumber = sqlErrorNumber;
        Context = context;
    }
}
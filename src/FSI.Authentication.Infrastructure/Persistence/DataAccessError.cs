namespace FSI.Authentication.Infrastructure.Persistence
{
    public enum DataAccessError
    {
        ConnectionOpenFailed,
        StoredProcedureNotFound,
        SqlTimeout,
        DeadlockVictim,
        UniqueViolation,
        ForeignKeyViolation,
        StringTruncation,
        LoginFailed,
        DatabaseNotFound,
        InvalidObject,
        Unknown
    }
}

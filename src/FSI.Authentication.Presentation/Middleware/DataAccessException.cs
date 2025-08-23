using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using FSI.Authentication.Infrastructure.Persistence; // DataAccessException / DataAccessError

namespace FSI.Authentication.Presentation.Middleware;

public sealed class DataAccessExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<DataAccessExceptionMiddleware> _logger;

    public DataAccessExceptionMiddleware(RequestDelegate next, ILogger<DataAccessExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext ctx)
    {
        try
        {
            await _next(ctx);
        }
        catch (DataAccessException ex)
        {
            var (status, code) = ex.Reason switch
            {
                DataAccessError.ConnectionOpenFailed => (503, "db_connection_failed"),
                DataAccessError.StoredProcedureNotFound => (500, "stored_procedure_missing"),
                DataAccessError.SqlTimeout => (504, "sql_timeout"),
                DataAccessError.DeadlockVictim => (503, "sql_deadlock"),
                DataAccessError.UniqueViolation => (409, "unique_violation"),
                DataAccessError.ForeignKeyViolation => (409, "foreign_key_violation"),
                DataAccessError.StringTruncation => (400, "string_truncation"),
                DataAccessError.LoginFailed => (401, "db_login_failed"),
                DataAccessError.DatabaseNotFound => (500, "database_not_found"),
                DataAccessError.InvalidObject => (500, "invalid_object"),
                _ => (500, "sql_unknown_error")
            };

            var traceId = ctx.TraceIdentifier;

            _logger.LogWarning(ex,
                "DataAccessException capturada. Status={Status} Code={Code} SqlError={SqlError} Context={Context} TraceId={TraceId}",
                status, code, ex.SqlErrorNumber, ex.Context, traceId);

            if (!ctx.Response.HasStarted)
            {
                ctx.Response.StatusCode = status;
                ctx.Response.ContentType = "application/json; charset=utf-8";

                var payload = new
                {
                    error = code,
                    message = ex.Message,
                    sqlErrorNumber = ex.SqlErrorNumber,
                    context = ex.Context,
                    path = ctx.Request.Path.Value,
                    traceId
                };

                await ctx.Response.WriteAsync(JsonSerializer.Serialize(payload));
            }
            // Se a resposta já começou, apenas registra e re-lança
        }
    }
}

// Extension para facilitar o uso
public static class DataAccessExceptionMiddlewareExtensions
{
    public static IApplicationBuilder UseDataAccessExceptionHandling(this IApplicationBuilder app)
        => app.UseMiddleware<FSI.Authentication.Presentation.Middleware.DataAccessExceptionMiddleware>();
}

using FSI.Authentication.Application.Exceptions;
using Microsoft.AspNetCore.Http;      // <- faltava
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;     // <- faltava
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;

namespace FSI.Authentication.Presentation.Middleware
{
    public sealed class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;

        public ExceptionHandlingMiddleware(RequestDelegate next) => _next = next;

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                var problem = MapToProblemDetails(context, ex);
                context.Response.ContentType = "application/problem+json";
                context.Response.StatusCode = problem.Status ?? (int)HttpStatusCode.InternalServerError;

                var payload = JsonSerializer.Serialize(problem, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
                await context.Response.WriteAsync(payload);
            }
        }

        private static ProblemDetails MapToProblemDetails(HttpContext ctx, Exception ex)
        {
            // RFC 7807 base
            var type = "https://httpstatuses.com/500";
            var title = "Erro interno";
            var status = (int)HttpStatusCode.InternalServerError;

            switch (ex)
            {
                case ValidationAppException:
                    type = "https://httpstatuses.com/400"; title = "Erro de validação"; status = 400; break;
                case UnauthorizedException:
                    type = "https://httpstatuses.com/401"; title = "Não autorizado"; status = 401; break;
                case NotFoundException:
                    type = "https://httpstatuses.com/404"; title = "Não encontrado"; status = 404; break;
                case ConflictException:
                    type = "https://httpstatuses.com/409"; title = "Conflito"; status = 409; break;
            }

            var correlationId = ctx.Items.TryGetValue("X-Correlation-Id", out var cid) ? cid?.ToString() : null;

            return new ProblemDetails
            {
                Type = type,
                Title = title,
                Status = status,
                Detail = ex.Message,
                Instance = ctx.Request.Path
            }.WithExtensions(new Dictionary<string, object?>
            {
                ["correlationId"] = correlationId
            });
        }
    }

    internal static class ProblemDetailsExtensions
    {
        public static ProblemDetails WithExtensions(this ProblemDetails p, IDictionary<string, object?> ext)
        {
            foreach (var kv in ext) p.Extensions[kv.Key] = kv.Value;
            return p;
        }
    }
}

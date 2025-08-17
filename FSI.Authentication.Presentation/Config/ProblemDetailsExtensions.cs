using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace FSI.Authentication.Presentation.Config
{
    public static class ProblemDetailsExtensions
    {
        public static IServiceCollection AddStandardProblemDetails(this IServiceCollection services)
        {
            services.AddProblemDetails(options =>
            {
                // Exemplo: acrescentar traceId automaticamente
                options.CustomizeProblemDetails = ctx =>
                {
                    var traceId = ctx.HttpContext?.TraceIdentifier;
                    if (!string.IsNullOrEmpty(traceId))
                        ctx.ProblemDetails.Extensions["traceId"] = traceId;
                };
            });

            return services;
        }
    }
}

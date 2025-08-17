using Microsoft.AspNetCore.Mvc;

namespace FSI.Authentication.Presentation.ProblemDetails
{
    public static class ProblemDetailsSetup
    {
        public static IServiceCollection AddStandardProblemDetails(this IServiceCollection services)
        {
            services.AddProblemDetails(options =>
            {
                options.CustomizeProblemDetails = ctx =>
                {
                    ctx.ProblemDetails.Extensions["traceId"] = ctx.HttpContext.TraceIdentifier;
                    if (ctx.HttpContext.Items.TryGetValue("X-Correlation-Id", out var cid))
                        ctx.ProblemDetails.Extensions["correlationId"] = cid?.ToString();
                };
            });
            return services;
        }
    }
}

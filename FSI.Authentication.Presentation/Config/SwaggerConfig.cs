using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;

namespace FSI.Authentication.Presentation.Config
{
    public static class SwaggerConfig
    {
        public static IServiceCollection AddApiSwagger(this IServiceCollection services)
        {
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "FSI Authentication API", Version = "v1" });

                var scheme = new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Description = "JWT Bearer token. Ex: Bearer {token}",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    BearerFormat = "JWT",
                    Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
                };

                c.AddSecurityDefinition("Bearer", scheme);
                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    [scheme] = Array.Empty<string>()
                });
            });

            return services;
        }
    }
}

using FSI.Authentication.Infrastructure.Security; 
using FSI.Authentication.Presentation.Config; 
using FSI.Authentication.Presentation.Filters;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Text;

namespace FSI.Authentication.Presentation.Config;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddPresentationDefaults(this IServiceCollection services)
    {
        services.AddControllers(o => o.Filters.Add<ModelStateValidationFilter>());
        services.AddStandardProblemDetails();
        services.AddApiSwagger();
        return services;
    }

    public static IServiceCollection AddJwtAuthentication(this IServiceCollection services, JwtOptions jwt)
    {
        services
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt.SigningKey));
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = jwt.Issuer,
                    ValidateAudience = true,
                    ValidAudience = jwt.Audience,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = key,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                };
            });

        services.AddAuthorization(options =>
        {
            options.AddPolicy("Users.Read", p => p.RequireClaim("perm", "users.read"));
        });

        return services;
    }
}

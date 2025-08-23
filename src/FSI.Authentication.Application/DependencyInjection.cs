using FSI.Authentication.Application.Interfaces;
using FSI.Authentication.Application.Interfaces.Services;
using FSI.Authentication.Application.Services;
using FSI.Authentication.Application.UseCases.ChangePassword;
using FSI.Authentication.Application.UseCases.Login;
using FSI.Authentication.Application.UseCases.RegisterUser;
using Microsoft.Extensions.DependencyInjection;
namespace FSI.Authentication.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<AuthenticationAppService>();
        services.AddScoped<LoginHandler>();
        services.AddScoped<RegisterUserHandler>();
        services.AddScoped<ChangePasswordHandler>();
        services.AddScoped<GeoLoggingAppService>();
        services.AddScoped<AdminAppService>();
        return services;
    }
}

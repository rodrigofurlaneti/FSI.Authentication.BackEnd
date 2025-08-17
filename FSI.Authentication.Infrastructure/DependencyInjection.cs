using Microsoft.Extensions.DependencyInjection;
using FSI.Authentication.Domain.Interfaces;
using FSI.Authentication.Domain.Abstractions.Messaging;
using FSI.Authentication.Domain.Abstractions.Security;
using FSI.Authentication.Infrastructure.Persistence;
using FSI.Authentication.Infrastructure.Repositories;
using FSI.Authentication.Infrastructure.Services;
using FSI.Authentication.Infrastructure.Outbox;
using FSI.Authentication.Infrastructure.Security;

namespace FSI.Authentication.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddPersistence(
        this IServiceCollection services,
        string connectionString)
    {
        services.AddSingleton<ISqlConnectionFactory>(_ => new SqlConnectionFactory(connectionString));
        services.AddScoped<DbSession>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        // Repositórios/Serviços
        services.AddScoped<IUserAccountRepository, UserAccountRepository>();
        services.AddScoped<IUserAccountService, UserAccountService>();

        return services;
    }

    public static IServiceCollection AddMessaging(this IServiceCollection services)
    {
        services.AddScoped<IOutbox, SqlOutbox>();
        services.AddScoped<IEventPublisher, EventPublisher>();
        services.AddSingleton<IMessageBus, SqlMessageBus>();
        return services;
    }

    public static IServiceCollection AddSecurity(this IServiceCollection services, JwtOptions jwtOptions)
    {
        services.AddSingleton(jwtOptions);                  // opções concretas
        services.AddSingleton<IPasswordHasher, Pbkdf2PasswordHasher>();
        services.AddSingleton<ITokenProvider, JwtTokenProvider>();
        return services;
    }
}

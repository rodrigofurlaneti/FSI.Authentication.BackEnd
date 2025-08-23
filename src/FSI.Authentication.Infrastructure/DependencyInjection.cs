using Microsoft.Extensions.DependencyInjection;
using FSI.Authentication.Application.Interfaces;
using FSI.Authentication.Domain.Abstractions.Messaging;
using FSI.Authentication.Infrastructure.Persistence;
using FSI.Authentication.Infrastructure.Repositories;
using FSI.Authentication.Infrastructure.Services;
using FSI.Authentication.Infrastructure.Outbox;
using FSI.Authentication.Infrastructure.Security;
using AppRepos = FSI.Authentication.Application.Interfaces.Repositories;
using AppServ = FSI.Authentication.Application.Interfaces.Services;
using AppMess = FSI.Authentication.Application.Interfaces.Messaging;

namespace FSI.Authentication.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddPersistence(
            this IServiceCollection services,
            string connectionString)
        {
            services.AddSingleton<ISqlConnectionFactory>(_ => new SqlConnectionFactory(connectionString));
            services.AddScoped<DbSession>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<AppRepos.IGeoLogRepository, GeoLogRepository>();
            // Repositórios/Serviços
            services.AddScoped<AppRepos.IUserAccountRepository, UserAccountRepository>();

            return services;
        }

        public static IServiceCollection AddMessaging(this IServiceCollection services)
        {
            services.AddScoped<IOutbox, SqlOutbox>();
            services.AddScoped<IEventPublisher, EventPublisher>();
            services.AddSingleton<AppMess.IMessageBus, SqlMessageBus>();
            return services;
        }

        public static IServiceCollection AddSecurity(this IServiceCollection services, JwtOptions jwtOptions)
        {
            services.AddSingleton<AppServ.IPasswordHasher, Pbkdf2PasswordHasher>();
            services.AddSingleton(jwtOptions);
            services.AddSingleton<AppServ.ITokenProvider, JwtTokenProvider>();

            services.AddScoped<UserAccountService>();

            services.AddScoped<FSI.Authentication.Domain.Services.IAuthDomainService>(
                sp => sp.GetRequiredService<UserAccountService>());

            services.AddScoped<FSI.Authentication.Domain.Interfaces.IUserAccountService>(
                sp => sp.GetRequiredService<UserAccountService>());
            return services;
        }
    }
}



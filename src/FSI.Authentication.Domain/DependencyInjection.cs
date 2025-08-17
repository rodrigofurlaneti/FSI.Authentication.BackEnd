using Microsoft.Extensions.DependencyInjection;
using FSI.Authentication.Domain.Abstractions;

namespace FSI.Authentication.Domain
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddDomain(this IServiceCollection services)
        {
            services.AddSingleton<IClock, SystemClock>();
            return services;
        }
    }
}

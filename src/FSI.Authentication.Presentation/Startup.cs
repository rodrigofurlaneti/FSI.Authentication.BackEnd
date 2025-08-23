using FSI.Authentication.Application;
using FSI.Authentication.Application.Interfaces.Services;
using FSI.Authentication.Application.Services;
// Nossas extensions:
using FSI.Authentication.Domain;
using FSI.Authentication.Infrastructure;
using FSI.Authentication.Infrastructure.Security;
using FSI.Authentication.Presentation.Config;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace FSI.Authentication.Presentation
{
    public sealed class Startup
    {
        private readonly IConfiguration _configuration;
        public Startup(IConfiguration configuration) => _configuration = configuration;

        public void ConfigureServices(IServiceCollection services)
        {
            var cs = _configuration.GetConnectionString("DefaultConnection")
                     ?? "Server=localhost,1433;Database=AuthBd;User Id=app_fsi;Password=SenhaForte!123;TrustServerCertificate=True;";

            var jwt = _configuration.GetSection("Jwt").Get<JwtOptions>()
                     ?? new JwtOptions { SigningKey = "troque-esta-chave", Issuer = "FSI.Auth", Audience = "FSI.API", ExpirationMinutes = 60 };

            services.AddCors(options =>
            {
                options.AddPolicy("GeoCors", builder =>
                {
                    builder
                        .WithOrigins("https://www.furlaneti.com/" , "https://www.furlaneti.com", "https://furlaneti.com")
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .SetIsOriginAllowedToAllowWildcardSubdomains()
                        .SetPreflightMaxAge(TimeSpan.FromHours(1));
                });
            });


            services
                .AddPresentationDefaults()
                .AddDomain()
                .AddApplication()
                .AddPersistence(cs)
                .AddMessaging()
                .AddSecurity(jwt)
                .AddJwtAuthentication(jwt);

            services.AddHttpClient<IGeoEnricher, GeoEnricherService>(c => c.Timeout = TimeSpan.FromSeconds(6));

        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {    // 1) Respeitar cabeçalhos do proxy (ARR/IIS)
            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
                // Opcional (endurecer):
                // KnownProxies = { System.Net.IPAddress.Loopback } // 127.0.0.1
            });

            app.UseSwagger();
            app.UseSwaggerUI();
            app.UseMiddleware<FSI.Authentication.Presentation.Middleware.CorrelationIdMiddleware>();
            app.UseMiddleware<FSI.Authentication.Presentation.Middleware.ExceptionHandlingMiddleware>();
            app.UseMiddleware<FSI.Authentication.Presentation.Middleware.DataAccessExceptionMiddleware>();

            if (!env.IsDevelopment())
                app.UseHttpsRedirection(); // só em produção
            app.UseRouting();
            app.UseCors("GeoCors");
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseEndpoints(e =>
            {
                e.MapControllers();
            });
        }
    }
}

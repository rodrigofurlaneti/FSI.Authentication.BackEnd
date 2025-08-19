using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

// Nossas extensions:
using FSI.Authentication.Domain;
using FSI.Authentication.Application;
using FSI.Authentication.Infrastructure;
using FSI.Authentication.Presentation.Config;
using FSI.Authentication.Infrastructure.Security;

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
                        .WithOrigins("http://127.0.0.1:5500", "http://localhost:5500") // frontend local
                        .AllowAnyHeader()
                        .AllowAnyMethod();
                    // .AllowCredentials() se precisar enviar cookies
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
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseMiddleware<FSI.Authentication.Presentation.Middleware.CorrelationIdMiddleware>();
            app.UseMiddleware<FSI.Authentication.Presentation.Middleware.ExceptionHandlingMiddleware>();

            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseCors("GeoCors");
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseEndpoints(e => e.MapControllers());
        }
    }
}

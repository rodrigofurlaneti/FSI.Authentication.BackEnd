using FSI.Authentication.Application.Handlers;
using FSI.Authentication.Application.Interfaces.Messaging;
using FSI.Authentication.Application.Interfaces.Repositories;
using FSI.Authentication.Application.Interfaces.Services;
using FSI.Authentication.Application.Pipeline; // ValidationBehavior<,>, TransactionBehavior<,>
// Application
using FSI.Authentication.Application.Services;
using FSI.Authentication.Application.UseCases.ChangePassword;
using FSI.Authentication.Application.UseCases.ChangeProfile;
using FSI.Authentication.Application.UseCases.GetProfile;
using FSI.Authentication.Application.UseCases.Login;
using FSI.Authentication.Application.UseCases.RegisterUser;
using FSI.Authentication.Application.Validators;
using FSI.Authentication.Infrastructure.Messaging;
using FSI.Authentication.Infrastructure.Outbox;
// Infrastructure
using FSI.Authentication.Infrastructure.Persistence;
using FSI.Authentication.Infrastructure.Repositories;
using FSI.Authentication.Infrastructure.Security;
// Presentation
using FSI.Authentication.Presentation.Config;
using FSI.Authentication.Presentation.ProblemDetails;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace FSI.Authentication.Presentation
{
    public sealed class Startup
    {
        private readonly IConfiguration _configuration;
        public Startup(IConfiguration configuration) => _configuration = configuration;

        public void ConfigureServices(IServiceCollection services)
        {
            // ======= Presentation =======
            services.AddControllers(options =>
            {
                options.Filters.Add<FSI.Authentication.Presentation.Filters.ModelStateValidationFilter>();
            });
            services.AddStandardProblemDetails();   // RFC7807 com traceId/correlationId
            services.AddApiSwagger();               // Swagger + esquema JWT

            // ======= Configurações (ConnectionString / JWT) =======
            var connectionString = _configuration.GetConnectionString("DefaultConnection")
                ?? "Server=localhost,1433;Database=AuthBd;User Id=app_fsi;Password=SenhaForte!123;TrustServerCertificate=True;";

            var jwtOptions = _configuration.GetSection("Jwt").Get<JwtOptions>()
                ?? new JwtOptions { SigningKey = "troque-esta-chave-super-secreta", Issuer = "FSI.Auth", Audience = "FSI.API", ExpirationMinutes = 60 };

            // ======= Infrastructure: Persistence / UoW =======
            services.AddSingleton<ISqlConnectionFactory>(_ => new SqlConnectionFactory(connectionString));
            services.AddScoped<DbSession>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            // Expor beginTx para TransactionBehavior<TReq,TRes>
            services.AddScoped<Func<CancellationToken, Task<IDisposable>>>(sp =>
                ct => sp.GetRequiredService<IUnitOfWork>().BeginAsync(ct)
            );

            // ======= Infrastructure: Repos / Outbox / Messaging / Security =======
            services.AddScoped<IUserAccountRepository, UserAccountRepository>();

            services.AddScoped<IOutbox, SqlOutbox>();
            services.AddScoped<IEventPublisher, EventPublisher>();

            services.AddSingleton<IMessageBus, SqlMessageBus>();

            services.AddSingleton<IPasswordHasher, Pbkdf2PasswordHasher>();
            services.AddSingleton(jwtOptions);
            services.AddSingleton<ITokenProvider, JwtTokenProvider>();

            // ======= Application: Services / UseCases / Validators =======
            services.AddScoped<AuthenticationAppService>();

            // UseCases (Handlers)
            services.AddScoped<RegisterUserHandler>();
            services.AddScoped<LoginHandler>();
            services.AddScoped<GetProfileHandler>();
            services.AddScoped<ChangePasswordHandler>();
            services.AddScoped<ChangeProfileHandler>();

            // Validators
            services.AddSingleton<IValidator<RegisterUserCommand>, RegisterUserValidator>();
            services.AddSingleton<IValidator<LoginCommand>, LoginValidator>();
            services.AddSingleton<IValidator<ChangePasswordCommand>, ChangePasswordValidator>();
            services.AddSingleton<IValidator<ChangeProfileCommand>, ChangeProfileValidator>();

            // Pipeline Behaviors (decorators do fluxo de casos de uso)
            services.AddScoped(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
            services.AddScoped(typeof(IPipelineBehavior<,>), typeof(TransactionBehavior<,>));

            // ======= AuthN/AuthZ (JWT Bearer) =======
            services
                .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.SigningKey));
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidIssuer = jwtOptions.Issuer,
                        ValidateAudience = true,
                        ValidAudience = jwtOptions.Audience,
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = key,
                        ValidateLifetime = true,
                        ClockSkew = TimeSpan.Zero
                    };
                });

            services.AddAuthorization(options =>
            {
                // Exemplo de policy por permissão
                options.AddPolicy("Users.Read", p => p.RequireClaim("perm", "users.read"));
            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            // Swagger no Dev
            if (env.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            // Middlewares de Observabilidade/Erros
            app.UseMiddleware<FSI.Authentication.Presentation.Middleware.CorrelationIdMiddleware>();
            app.UseMiddleware<FSI.Authentication.Presentation.Middleware.ExceptionHandlingMiddleware>();

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}

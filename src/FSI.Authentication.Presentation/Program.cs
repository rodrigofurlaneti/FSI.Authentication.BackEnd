using FSI.Authentication.Presentation;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Hosting.WindowsServices; // <- pacote acima
using Microsoft.Extensions.Logging;

var builder = WebApplication.CreateBuilder(args);

// REMOVER estas linhas no cenário IIS in-process:
//builder.Host.UseWindowsService();

builder.Host.UseDefaultServiceProvider(o =>
{
    o.ValidateOnBuild = true;
    o.ValidateScopes = true;
});

// REMOVER estas linhas no cenário IIS in-process:
//builder.WebHost.UseUrls("http://localhost:5000");

builder.Logging.AddEventLog();

// Usa a classe Startup para DI/pipeline
var startup = new Startup(builder.Configuration);
startup.ConfigureServices(builder.Services);

var app = builder.Build();
startup.Configure(app, app.Environment);

app.Run();

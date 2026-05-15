using DeslandesApp.API.Configurations;
using DeslandesApp.API.Middlewares;
using DeslandesApp.Domain.Extensions;
using DeslandesApp.Domain.Helpers;
using DeslandesApp.Domain.Interfaces.Services;
using DeslandesApp.Infra.Data.Extensions;

using Hangfire;
using Hangfire.SqlServer;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Extensions.FileProviders;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

using System.Text;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

//
// ================================
// APPSETTINGS
// ================================
//
builder.Configuration
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile(
        "appsettings.json",
        optional: false,
        reloadOnChange: true
    )
    .AddJsonFile(
        $"appsettings.{builder.Environment.EnvironmentName}.json",
        optional: true,
        reloadOnChange: true
    )
    .AddEnvironmentVariables();

Console.WriteLine(
    $">>> Ambiente atual: {builder.Environment.EnvironmentName}"
);

//
// ================================
// SERVICES
// ================================
//
builder.Services.AddHttpContextAccessor();

//
// ================================
// CONTROLLERS
// ================================
//
builder.Services
    .AddControllers(config =>
    {
        var policy =
            new Microsoft.AspNetCore.Authorization.AuthorizationPolicyBuilder()
                .RequireAuthenticatedUser()
                .Build();

        config.Filters.Add(
            new AuthorizeFilter(policy)
        );
    })
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy =
            JsonNamingPolicy.CamelCase;

        options.JsonSerializerOptions.DictionaryKeyPolicy =
            JsonNamingPolicy.CamelCase;
    });

//
// ================================
// SWAGGER
// ================================
//
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc(
        "v1",
        new OpenApiInfo
        {
            Title = "DeslandesApp API",
            Version = "v1",
            Description = "API REST .NET com EntityFramework"
        }
    );

    // JWT
    c.AddSecurityDefinition(
        "Bearer",
        new OpenApiSecurityScheme
        {
            Name = "Authorization",
            Type = SecuritySchemeType.Http,
            Scheme = "Bearer",
            BearerFormat = "JWT",
            In = ParameterLocation.Header,
            Description =
                "Insira o token JWT assim: Bearer {token}"
        }
    );

    c.AddSecurityRequirement(
        new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference =
                        new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                },
                Array.Empty<string>()
            }
        }
    );
});

//
// ================================
// ENTITY FRAMEWORK
// ================================
//
builder.Services.AddEntityFramework(
    builder.Configuration
);

DependencyInjectionConfiguration.Configure(
    builder.Services
);

//
// ================================
// DOMAIN SERVICES
// ================================
//
builder.Services.AddDomainService();

builder.Services.AddScoped<FunctionsHelper>();

//
// ================================
// JWT + CORS
// ================================
//
JwtConfiguration.Configure(
    builder.Services
);

CorsConfiguration.Configure(
    builder.Services
);

//
// ================================
// HANGFIRE
// ================================
//
builder.Services.AddHangfire(config =>
    config
        .SetDataCompatibilityLevel(
            CompatibilityLevel.Version_180
        )
        .UseSimpleAssemblyNameTypeSerializer()
        .UseRecommendedSerializerSettings()
        .UseSqlServerStorage(
            builder.Configuration.GetConnectionString(
                "DeslandesApp"
            )
        )
);

builder.Services.AddHangfireServer();

//
// ================================
// APP
// ================================
//
var app = builder.Build();

//
// ================================
// MIDDLEWARES
// ================================
//
app.UseDeveloperExceptionPage();

app.UseMiddleware<ExceptionMiddleware>();

//
// ================================
// SWAGGER
// ================================
//
app.UseSwagger();

app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint(
        "/swagger/v1/swagger.json",
        "DeslandesApp API V1"
    );

    // swagger na raiz
    c.RoutePrefix = string.Empty;
});

//
// ================================
// HANGFIRE DASHBOARD
// ================================
//
app.UseHangfireDashboard("/hangfire");

//
// ================================
// PIPELINE
// ================================
//
app.UseRouting();

app.UseCors(
    CorsConfiguration.PolicyName
);

//
// ================================
// STATIC FILES (WWWROOT)
// ================================
//
app.UseStaticFiles();

//
// ================================
// UPLOADS
// ================================
//
var uploadsPath = Path.Combine(
    builder.Environment.WebRootPath,
    "uploads"
);

// cria pasta uploads se não existir
if (!Directory.Exists(uploadsPath))
{
    Directory.CreateDirectory(uploadsPath);
}

// expõe /uploads
app.UseStaticFiles(
    new StaticFileOptions
    {
        FileProvider =
            new PhysicalFileProvider(
                uploadsPath
            ),

        RequestPath = "/uploads"
    }
);

//
// ================================
// AUTH
// ================================
//
app.UseAuthentication();

app.UseAuthorization();

//
// ================================
// CONTROLLERS
// ================================
//
app.MapControllers();

//
// ================================
// HANGFIRE JOBS
// ================================
//
RecurringJob.AddOrUpdate<IEventoService>(
    "atualizar-status-eventos",
    x => x.AtualizarStatusAutomatico(),
    Cron.Hourly
);

RecurringJob.AddOrUpdate<ITarefaService>(
    "atualizar-status-tarefas",
    x => x.AtualizarStatusTarefasAutomatico(),
    Cron.Hourly
);

//
// ================================
// SPA FALLBACK (Angular)
// ================================
//

// se precisar depois:
// app.MapFallbackToFile("index.html");

//
// ================================
// RUN
// ================================
//
app.Run();

//
// ================================
// TESTES
// ================================
//
public partial class Program { }
using System.Text.Json.Serialization;
using Core.Configs;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Presentation.Extensions;
using Presentation.Middlewares;

var builder = WebApplication.CreateBuilder(args);

builder.Services.RegisterConfigurations(builder.Configuration);
var appConfig = new AppConfig();
builder.Configuration.GetSection(nameof(AppConfig)).Bind(appConfig);

builder.ConfigureSerilog(appConfig);

builder.Services
    .RegisterOpenTelemetry(appConfig)
    .RegisterDataAccess(builder.Configuration)
    .RegisterServices()
    .RegisterValidators()
    .RegisterExceptionHandlers()
    .RegisterHealthChecks()
    .AddHttpClient()
    .AddProblemDetails()
    .AddEndpointsApiExplorer()
    .AddSwaggerGen()
    .AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
        options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
    });

var app = builder.Build();

app.UseExceptionHandler();
app.UseMiddleware<CorrelationIdMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();

app.MapHealthChecks("/health");
app.MapHealthChecks("/health/detailed", new HealthCheckOptions
{
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});

app.MapHealthChecks("/healthz/ready", new HealthCheckOptions
{
    Predicate = healthCheck => healthCheck.Tags.Contains("ready")
});

app.MapHealthChecks("/healthz/live", new HealthCheckOptions
{
    Predicate = _ => false
});

app.Run();
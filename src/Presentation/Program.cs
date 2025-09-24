using System.Text.Json.Serialization;
using Core.Configs;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Presentation.Extensions;
using Presentation.Filters;
using Presentation.Middlewares;
using QuestPDF.Infrastructure;

QuestPDF.Settings.License = LicenseType.Community;

var builder = WebApplication.CreateBuilder(args);

builder.Services.RegisterConfigurations(builder.Configuration);
var appConfig = new AppConfig();
builder.Configuration.GetSection(nameof(AppConfig)).Bind(appConfig);

builder.ConfigureSerilog(appConfig);

builder.Services
    .RegisterOpenTelemetry(appConfig)
    .RegisterDataAccess(builder.Configuration)
    .RegisterBackgroundServices(builder.Configuration)
    .RegisterJobs(builder.Configuration)
    .RegisterAwsServices(builder.Configuration)
    .RegisterServices()
    .RegisterValidators()
    .RegisterExceptionHandlers()
    .RegisterHealthChecks()
    .AddHttpClient()
    .AddProblemDetails()
    .AddEndpointsApiExplorer()
    .AddSwaggerGen()
    .AddControllers(options =>
    {
        options.Filters.Add<HeaderValidationFilter>();
    })
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = null;
        options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
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
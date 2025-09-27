using System.Net;
using System.Net.Mime;
using System.Text.Json;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using OpenTelemetry.Trace;
using ILogger = Serilog.ILogger;

namespace Presentation.ExceptionHandlers;

public class GlobalExceptionHandler(
    IHostEnvironment env,
    IOptions<JsonOptions>? jsonOptions,
    ILogger logger) : IExceptionHandler
{
    private readonly JsonSerializerOptions _jsonSerializerOptions = jsonOptions?.Value.JsonSerializerOptions ?? new JsonSerializerOptions(JsonSerializerDefaults.Web);

    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        Tracer.CurrentSpan.SetStatus(Status.Error);
        Tracer.CurrentSpan.RecordException(exception);
        logger.Error(exception, "Exception occurred: {Message}", exception.Message);

        await WriteHttpResponseAsync(httpContext, exception, cancellationToken);

        return true;
    }

    private Task WriteHttpResponseAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        httpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
        httpContext.Response.ContentType = MediaTypeNames.Application.Json;

        var problemDetails = new ProblemDetails
        {
            Status = StatusCodes.Status500InternalServerError,
            Detail = "An unexpected error occurred.",
            Title = "Internal Server Error",
            Instance = httpContext.Request.Path
        };

        if (env.IsDevelopment())
        {
            problemDetails.Extensions["exceptionMessage"] = exception.Message;
            problemDetails.Extensions["stackTrace"] = exception.StackTrace;
        }

        return httpContext.Response.WriteAsJsonAsync(problemDetails, _jsonSerializerOptions, cancellationToken);
    }
}
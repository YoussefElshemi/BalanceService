using System.Net;
using System.Net.Mime;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using OpenTelemetry.Trace;
using ILogger = Serilog.ILogger;

namespace Presentation.ExceptionHandlers;

internal sealed class GlobalExceptionHandler(ILogger logger) : IExceptionHandler
{
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

    private static Task WriteHttpResponseAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
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

        return httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);
    }
}
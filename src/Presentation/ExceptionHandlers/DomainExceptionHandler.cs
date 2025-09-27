using System.Net.Mime;
using System.Text.Json;
using Core.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using OpenTelemetry.Trace;

namespace Presentation.ExceptionHandlers;

public class DomainExceptionHandler(IOptions<JsonOptions>? jsonOptions) : IExceptionHandler
{
    private readonly JsonSerializerOptions _jsonSerializerOptions = jsonOptions?.Value.JsonSerializerOptions ?? new JsonSerializerOptions(JsonSerializerDefaults.Web);

    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        if (exception is not DomainException domainException)
        {
            return false;
        }

        Tracer.CurrentSpan.SetStatus(Status.Error);
        Tracer.CurrentSpan.RecordException(exception);
        
        await WriteHttpResponseAsync(httpContext, domainException, cancellationToken);

        return true;
    }

    private Task WriteHttpResponseAsync(
        HttpContext httpContext,
        DomainException exception,
        CancellationToken cancellationToken)
    {
        httpContext.Response.StatusCode = (int)exception.GetStatusCode();
        httpContext.Response.ContentType = MediaTypeNames.Application.Json;

        var problemDetails = new ProblemDetails
        {
            Type = exception.GetType().Name,
            Title = exception.GetTitle(),
            Detail = exception.Message,
            Instance = httpContext.Request.Path,
            Status = httpContext.Response.StatusCode
        };

        return httpContext.Response.WriteAsJsonAsync(problemDetails, _jsonSerializerOptions, cancellationToken);
    }

}
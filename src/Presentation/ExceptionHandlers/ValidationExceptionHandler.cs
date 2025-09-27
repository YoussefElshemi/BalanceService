using System.Net;
using System.Net.Mime;
using System.Text.Json;
using FluentValidation;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using OpenTelemetry.Trace;

namespace Presentation.ExceptionHandlers;

public class ValidationExceptionHandler(IOptions<JsonOptions>? jsonOptions) : IExceptionHandler
{
    private readonly JsonSerializerOptions _jsonSerializerOptions = jsonOptions?.Value.JsonSerializerOptions ?? new JsonSerializerOptions(JsonSerializerDefaults.Web);

    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        if (exception is not ValidationException validationException)
        {
            return false;
        }

        Tracer.CurrentSpan.SetStatus(Status.Error);
        Tracer.CurrentSpan.RecordException(exception);
        
        await WriteHttpResponseAsync(httpContext, validationException, cancellationToken);

        return true;
    }

    private Task WriteHttpResponseAsync(
        HttpContext httpContext,
        ValidationException exception,
        CancellationToken cancellationToken)
    {
        httpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;
        httpContext.Response.ContentType = MediaTypeNames.Application.Json;

        var validationProblemDetails = new ValidationProblemDetails
        {
            Type = exception.GetType().Name,
            Title = nameof(HttpStatusCode.BadRequest),
            Detail = "Please refer to the errors property for additional details.",
            Instance = httpContext.Request.Path,
            Status = httpContext.Response.StatusCode,
            Errors = exception.Errors
                .GroupBy(x => x.PropertyName)
                .ToDictionary(x => x.Key, x =>
                    x.Select(y => y.ErrorMessage).ToArray())
        };

        return httpContext.Response.WriteAsJsonAsync(validationProblemDetails, _jsonSerializerOptions, cancellationToken);
    }

}
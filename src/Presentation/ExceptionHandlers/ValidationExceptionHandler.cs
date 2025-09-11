using System.Net;
using System.Net.Mime;
using FluentValidation;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using OpenTelemetry.Trace;

namespace Presentation.ExceptionHandlers;

internal sealed class ValidationExceptionHandler : IExceptionHandler
{
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

    private static Task WriteHttpResponseAsync(
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

        return httpContext.Response.WriteAsJsonAsync(validationProblemDetails, cancellationToken);
    }

}
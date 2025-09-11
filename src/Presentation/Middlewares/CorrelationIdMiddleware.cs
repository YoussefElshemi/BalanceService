using Presentation.Constants;

namespace Presentation.Middlewares;

public class CorrelationIdMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context)
    {
        context.Response.OnStarting(() =>
        {
            if (context.Response.HasStarted)
            {
                return Task.CompletedTask;
            }

            if (context.Request.Headers.TryGetValue(HeaderNames.CorrelationId, out var correlationId))
            {
                context.Response.Headers.Append(HeaderNames.CorrelationId, correlationId);
            }
            else
            {
                context.Response.Headers.Append(HeaderNames.CorrelationId, Guid.NewGuid().ToString());
            }

            return Task.CompletedTask;
        });

        await next(context);
    }
}
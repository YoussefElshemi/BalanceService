using System.Diagnostics;
using Infrastructure.Constants;

namespace Infrastructure.OpenTelemetry;

public static class OpenTelemetry
{
    public static readonly ActivitySource MyActivitySource = new(OpenTelemetryConstants.ServiceName);
}
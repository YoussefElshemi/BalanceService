using Environment = Core.Enums.Environment;

namespace Core.Configs;

public record AppConfig
{
    public string Version { get; init; } = null!;
    public Environment Environment { get; init; }
    public ObservabilityConfig ObservabilityConfig { get; init; } = null!;
    public HoldExpiryConfig HoldExpiryConfig { get; init; } = null!;
    public AccountUpdateNotificationConfig AccountUpdateNotificationConfig { get; init; } = null!;
}
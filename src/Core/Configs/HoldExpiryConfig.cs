namespace Core.Configs;

public record HoldExpiryConfig
{
    public bool Enabled { get; init; }
    public TimeSpan Interval { get; init; }
}
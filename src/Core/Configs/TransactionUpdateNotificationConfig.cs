namespace Core.Configs;

public record TransactionUpdateNotificationConfig : IUpdateNotificationConfig
{
    public bool Enabled { get; init; }
    public TimeSpan Interval { get; init; }
    public int MaxCount { get; init; }
    public string TopicArn { get; init; } = null!;
}
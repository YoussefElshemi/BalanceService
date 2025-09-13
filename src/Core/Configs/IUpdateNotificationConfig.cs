namespace Core.Configs;

public interface IUpdateNotificationConfig
{
    public bool Enabled { get; init; }
    public TimeSpan Interval { get; init; }
    public int MaxCount { get; init; }
    public string TopicArn { get; init; }
}
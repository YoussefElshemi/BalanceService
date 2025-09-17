namespace Core.Configs;

public interface IUpdateNotificationConfig
{
    public bool Enabled { get; }
    public TimeSpan Interval { get; }
    public int MaxCount { get; }
    public string TopicArn { get; }
}
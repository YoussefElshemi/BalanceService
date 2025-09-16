namespace Core.Configs;

public record InterestAccrualJob
{
    public bool Enabled { get; init; }
    public TimeSpan Interval { get; init; }
    public string JobName { get; init; } = null!;
    public string CronExpression { get; init; } = null!;
    public TimeSpan InterestEligibilityLag { get; init; }
}
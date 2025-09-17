namespace Core.Configs;

public record InterestAccrualJobConfig : IJobConfig
{
    public bool Enabled { get; init; }
    public string JobName { get; init; } = null!;
    public string CronExpression { get; init; } = null!;
    public TimeSpan InterestEligibilityLag { get; init; }
}
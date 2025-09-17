namespace Core.Configs;

public interface IJobConfig
{
    public string JobName { get; }
    public bool Enabled { get; }
    public string CronExpression { get; }
}
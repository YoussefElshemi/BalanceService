namespace Core.Interfaces;

public interface IJobExecutor
{
    Task ExecuteAsync(CancellationToken cancellationToken);
}
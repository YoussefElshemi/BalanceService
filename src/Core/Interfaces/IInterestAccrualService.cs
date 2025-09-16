namespace Core.Interfaces;

public interface IInterestAccrualService
{
    Task AccrueMissingDaysAsync(CancellationToken cancellationToken);
}
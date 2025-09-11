using Core.Models;

namespace Core.Interfaces;

public interface ITransferService
{
    Task<Transfer> CreateAsync(CreateTransferRequest createTransferRequest, CancellationToken cancellationToken);
}
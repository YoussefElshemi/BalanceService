using Core.Enums;
using Core.ValueObjects;

namespace Core.Interfaces;

public interface IAccountRulesService
{
    Task ThrowIfNotAllowedAsync(AccountId accountId, AccountOperationType operation, CancellationToken cancellationToken);
    void ThrowIfNotAllowed(AccountStatus accountStatus, AccountOperationType operation);
}
using Core.Enums;
using Core.Exceptions;
using Core.Interfaces;
using Core.ValueObjects;

namespace Core.Services;

public class AccountRulesService(IAccountRepository accountRepository) : IAccountRulesService
{
    private static readonly Dictionary<AccountStatus, HashSet<AccountOperationType>> Allowed = new()
    {
        // --- Pending Activation ---
        [AccountStatus.PendingActivation] =
        [
            AccountOperationType.ActivateAccount,
            AccountOperationType.DeleteAccount
        ],

        // --- Active ---
        [AccountStatus.Active] =
        [
            AccountOperationType.DebitTransaction,
            AccountOperationType.CreditTransaction,
            AccountOperationType.AccrueInterest,
            AccountOperationType.CreateHold,
            AccountOperationType.ReleaseHold,
            AccountOperationType.SettleHold,
            AccountOperationType.FreezeAccount,
            AccountOperationType.CloseAccount
        ],

        // --- Frozen ---
        [AccountStatus.Frozen] =
        [
            AccountOperationType.CreditTransaction,
            AccountOperationType.AccrueInterest,
            AccountOperationType.ReleaseHold,
            AccountOperationType.SettleHold,
            AccountOperationType.ActivateAccount,
            AccountOperationType.CloseAccount
        ],

        // --- Dormant ---
        [AccountStatus.Dormant] =
        [
            AccountOperationType.CreditTransaction,
            AccountOperationType.AccrueInterest,
            AccountOperationType.ActivateAccount,
            AccountOperationType.CloseAccount
        ],

        // --- Restricted ---
        [AccountStatus.Restricted] =
        [
            AccountOperationType.CreditTransaction,
            AccountOperationType.AccrueInterest,
            AccountOperationType.ActivateAccount,
            AccountOperationType.CloseAccount
        ],

        // --- Suspended ---
        [AccountStatus.Suspended] =
        [
            AccountOperationType.ActivateAccount,
            AccountOperationType.CloseAccount
        ],

        // --- Pending Closure ---
        [AccountStatus.PendingClosure] =
        [
            AccountOperationType.DebitTransaction,
            AccountOperationType.CreditTransaction,
            AccountOperationType.AccrueInterest,
            AccountOperationType.CreateHold,
            AccountOperationType.ReleaseHold,
            AccountOperationType.SettleHold,
            AccountOperationType.ActivateAccount,
            AccountOperationType.CloseAccount
        ]
    };

    public async Task ThrowIfNotAllowedAsync(AccountId accountId, AccountOperationType operation, CancellationToken cancellationToken)
    {
        var accountStatus = await accountRepository.GetStatusByIdAsync(accountId, cancellationToken)
                            ?? throw new NotFoundException();

        if (!IsAllowed(accountStatus, operation))
        {
            throw new AccountOperationForbiddenException(accountStatus, operation);
        }
    }

    public void ThrowIfNotAllowed(AccountStatus accountStatus, AccountOperationType operation)
    {
        if (!IsAllowed(accountStatus, operation))
        {
            throw new AccountOperationForbiddenException(accountStatus, operation);
        }
    }

    public static bool IsAllowed(AccountStatus accountStatus, AccountOperationType operation)
    {
        return Allowed.TryGetValue(accountStatus, out var ops) && ops.Contains(operation);
    }
}

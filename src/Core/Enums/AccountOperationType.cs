namespace Core.Enums;

public enum AccountOperationType
{
    // Transactions
    DebitTransaction = 1,
    CreditTransaction,

    // Holds
    CreateHold,
    ReleaseHold,
    SettleHold,

    // Account
    ActivateAccount,
    FreezeAccount,
    CloseAccount,
    DeleteAccount,
    
    // Interest
    AccrueInterest
}
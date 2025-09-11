namespace Core.Enums;

/// <summary>
/// Represents the lifecycle and operational state of an account.
/// </summary>
public enum AccountStatus
{
    /// <summary>
    /// Active
    /// ✅ Debits allowed
    /// ✅ Credits allowed
    /// ✅ Holds allowed
    /// ✅ Transfers allowed
    /// ✅ Can be closed (if balance = 0 and no pending holds)
    /// </summary>
    Active = 1,

    /// <summary>
    /// Frozen
    /// ❌ Debits blocked
    /// ✅ Credits may be allowed (business rule)
    /// ❌ Holds not allowed
    /// ❌ Transfers not allowed
    /// ✅ Can be unfrozen
    /// </summary>
    Frozen,

    /// <summary>
    /// Closed
    /// ❌ Debits not allowed
    /// ❌ Credits not allowed
    /// ❌ Holds not allowed
    /// ❌ Transfers not allowed
    /// ✅ Balance must be 0 before closure
    /// ✅ Historical data read-only
    /// </summary>
    Closed,

    /// <summary>
    /// PendingActivation
    /// ❌ Debits blocked
    /// ❌ Credits blocked
    /// ❌ Holds blocked
    /// ❌ Transfers blocked
    /// ✅ Can transition to Active once verification/KYC completed
    /// </summary>
    PendingActivation,

    /// <summary>
    /// Dormant
    /// ❌ Debits blocked
    /// ❌ Credits usually blocked (sometimes inbound credits allowed to wake account)
    /// ❌ Holds blocked
    /// ❌ Transfers blocked
    /// ✅ Can transition to Active after reactivation process
    /// </summary>
    Dormant,

    /// <summary>
    /// Suspended
    /// ❌ Debits blocked
    /// ❌ Credits blocked
    /// ❌ Holds blocked
    /// ❌ Transfers blocked
    /// ✅ Can be re-activated manually by admin/policy
    /// </summary>
    Suspended,

    /// <summary>
    /// Restricted
    /// ❌ Debits restricted (blocked or capped)
    /// ✅ Credits allowed (e.g., salary deposits still possible)
    /// ❌ Holds limited or blocked
    /// ❌ Transfers limited
    /// ✅ Rules depend on compliance/legal constraints
    /// </summary>
    Restricted,

    /// <summary>
    /// PendingClosure
    /// ❌ Debits blocked
    /// ❌ Credits blocked
    /// ❌ Holds blocked
    /// ❌ Transfers blocked
    /// ✅ Only operation allowed = settle pending transactions and zero balance
    /// ✅ Once balance = 0, transitions to Closed
    /// </summary>
    PendingClosure
}

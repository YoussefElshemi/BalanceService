namespace Infrastructure.Entities;

public record AccountEntity : DeletableBaseEntity
{
    public required Guid AccountId { get; init; }
    public required string AccountName { get; set; }
    public required string CurrencyCode { get; set; }

    /// <summary>
    /// Ledger balance (a.k.a. book/current balance) = sum of all posted transactions.
    /// </summary>
    public required decimal LedgerBalance { get; init; }

    /// <summary>
    /// Available balance = LedgerBalance + pending credits - pending debits - holds.
    /// This is what the user can spend right now.
    /// </summary>
    public required decimal AvailableBalance { get; init; }

    /// <summary>
    /// Pending balance = total of transactions that are authorized but not yet posted.
    /// </summary>
    public required decimal PendingBalance { get; init; }

    /// <summary>
    /// On-hold balance = funds locked due to disputes, fraud, or compliance holds.
    /// </summary>
    public required decimal HoldBalance { get; init; }

    /// <summary>
    /// Minimum balance requirement (not spendable).
    /// </summary>
    public required decimal MinimumRequiredBalance { get; set; }

    public required int AccountTypeId { get; set; }
    public required int AccountStatusId { get; set; }
    public required string? Metadata { get; set; }
    public required Guid? ParentAccountId { get; set; }

    public AccountEntity? ParentAccountEntity { get; init; }
    public ICollection<AccountEntity> ChildAccountEntities { get; init; } = null!;
    public AccountTypeEntity AccountTypeEntity { get; init; } = null!;
    public AccountStatusEntity AccountStatusEntity { get; init; } = null!;
    public ICollection<TransactionEntity>? TransactionEntities { get; init; }
    public ICollection<HoldEntity>? HoldEntities { get; init; }
    public ICollection<InterestAccrualEntity>? InterestAccrualEntities { get; init; }
    public ICollection<InterestProductAccountLinkEntity>? InterestProductAccountLinkEntities { get; init; }
}

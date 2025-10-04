namespace Core.Enums;

public enum StatementStatus
{
    Unknown = 0,

    TransactiomDraft = 1,
    TramsactiomPosted = 2,
    TransactionReversed = 3,

    HoldActive = 100,
    HoldReleased = 101,
    HoldSettled = 102,
    HoldExpired = 103
}

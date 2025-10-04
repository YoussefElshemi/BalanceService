namespace Core.Enums;

public enum StatementStatus
{
    Unknown = 0,

    Draft = 1,
    Posted = 2,
    Reversed = 3,

    Active = 100,
    Released = 101,
    Settled = 102,
    Expired = 103
}

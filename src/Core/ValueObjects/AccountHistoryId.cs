namespace Core.ValueObjects;

public readonly record struct AccountHistoryId
{
    public AccountHistoryId(Guid value)
    {
        Value = value;
    }

    private Guid Value { get; }

    public static implicit operator Guid(AccountHistoryId valueObject)
    {
        return valueObject.Value;
    }

    public override string ToString()
    {
        return Value.ToString();
    }
}
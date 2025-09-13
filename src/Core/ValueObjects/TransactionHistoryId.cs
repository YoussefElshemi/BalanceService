namespace Core.ValueObjects;

public readonly record struct TransactionHistoryId
{
    public TransactionHistoryId(Guid value)
    {
        Value = value;
    }

    private Guid Value { get; }

    public static implicit operator Guid(TransactionHistoryId valueObject)
    {
        return valueObject.Value;
    }

    public override string ToString()
    {
        return Value.ToString();
    }
}
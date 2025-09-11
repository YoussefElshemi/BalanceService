namespace Core.ValueObjects;

public readonly record struct TransactionDescription
{
    public TransactionDescription(string value)
    {
        Value = value;
    }

    private string Value { get; }

    public static implicit operator string(TransactionDescription valueObject)
    {
        return valueObject.Value;
    }

    public override string ToString()
    {
        return Value;
    }
}
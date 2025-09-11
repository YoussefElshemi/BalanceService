namespace Core.ValueObjects;

public readonly record struct TransactionReference
{
    public TransactionReference(string value)
    {
        Value = value;
    }

    private string Value { get; }

    public static implicit operator string(TransactionReference valueObject)
    {
        return valueObject.Value;
    }

    public override string ToString()
    {
        return Value;
    }
}
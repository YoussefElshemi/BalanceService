namespace Core.ValueObjects;

public readonly record struct TransferDescription
{
    public TransferDescription(string value)
    {
        Value = value;
    }

    private string Value { get; }

    public static implicit operator string(TransferDescription valueObject)
    {
        return valueObject.Value;
    }

    public override string ToString()
    {
        return Value;
    }
}
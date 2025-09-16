namespace Core.ValueObjects;

public readonly record struct InterestProductName
{
    public InterestProductName(string value)
    {
        Value = value;
    }

    private string Value { get; }

    public static implicit operator string(InterestProductName valueObject)
    {
        return valueObject.Value;
    }

    public override string ToString()
    {
        return Value;
    }
}
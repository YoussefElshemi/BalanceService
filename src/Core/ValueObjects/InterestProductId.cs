namespace Core.ValueObjects;

public readonly record struct InterestProductId
{
    public InterestProductId(Guid value)
    {
        Value = value;
    }

    private Guid Value { get; }

    public static implicit operator Guid(InterestProductId valueObject)
    {
        return valueObject.Value;
    }

    public override string ToString()
    {
        return Value.ToString();
    }
}
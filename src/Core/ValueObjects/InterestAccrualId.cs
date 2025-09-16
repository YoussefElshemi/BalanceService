namespace Core.ValueObjects;

public readonly record struct InterestAccrualId
{
    public InterestAccrualId(Guid value)
    {
        Value = value;
    }

    private Guid Value { get; }

    public static implicit operator Guid(InterestAccrualId valueObject)
    {
        return valueObject.Value;
    }

    public override string ToString()
    {
        return Value.ToString();
    }
}
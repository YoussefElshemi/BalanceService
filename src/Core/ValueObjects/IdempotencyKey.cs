namespace Core.ValueObjects;

public readonly record struct IdempotencyKey
{
    public IdempotencyKey(Guid value)
    {
        Value = value;
    }

    private Guid Value { get; }

    public static implicit operator Guid(IdempotencyKey valueObject)
    {
        return valueObject.Value;
    }

    public override string ToString()
    {
        return Value.ToString();
    }
}
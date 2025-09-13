namespace Core.ValueObjects;

public readonly record struct HoldHistoryId
{
    public HoldHistoryId(Guid value)
    {
        Value = value;
    }

    private Guid Value { get; }

    public static implicit operator Guid(HoldHistoryId valueObject)
    {
        return valueObject.Value;
    }

    public override string ToString()
    {
        return Value.ToString();
    }
}
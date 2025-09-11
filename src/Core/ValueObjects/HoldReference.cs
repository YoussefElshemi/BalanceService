namespace Core.ValueObjects;

public readonly record struct HoldReference
{
    public HoldReference(string value)
    {
        Value = value;
    }

    private string Value { get; }

    public static implicit operator string(HoldReference valueObject)
    {
        return valueObject.Value;
    }

    public override string ToString()
    {
        return Value;
    }
}
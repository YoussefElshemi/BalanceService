namespace Core.ValueObjects;

public readonly record struct PageNumber
{
    public PageNumber(int value)
    {
        Value = value;
    }

    private int Value { get; }

    public static implicit operator int(PageNumber valueObject)
    {
        return valueObject.Value;
    }

    public override string ToString()
    {
        return Value.ToString();
    }
}
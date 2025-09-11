namespace Core.ValueObjects;

public readonly record struct PageSize
{
    public PageSize(int value)
    {
        Value = value;
    }

    private int Value { get; }

    public static implicit operator int(PageSize valueObject)
    {
        return valueObject.Value;
    }

    public override string ToString()
    {
        return Value.ToString();
    }
}
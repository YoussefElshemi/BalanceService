using System.Globalization;

namespace Core.ValueObjects;

public readonly record struct HoldBalance
{
    public HoldBalance(decimal value)
    {
        Value = value;
    }

    private decimal Value { get; }

    public static implicit operator decimal(HoldBalance valueObject)
    {
        return valueObject.Value;
    }

    public override string ToString()
    {
        return Value.ToString(CultureInfo.InvariantCulture);
    }
}
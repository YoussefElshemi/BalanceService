using System.Globalization;

namespace Core.ValueObjects;

public readonly record struct HoldAmount
{
    public HoldAmount(decimal value)
    {
        Value = value;
    }

    private decimal Value { get; }

    public static implicit operator decimal(HoldAmount valueObject)
    {
        return valueObject.Value;
    }

    public override string ToString()
    {
        return Value.ToString(CultureInfo.InvariantCulture);
    }
}
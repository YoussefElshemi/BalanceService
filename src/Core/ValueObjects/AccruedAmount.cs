using System.Globalization;

namespace Core.ValueObjects;

public readonly record struct AccruedAmount
{
    public AccruedAmount(decimal value)
    {
        Value = value;
    }

    private decimal Value { get; }

    public static implicit operator decimal(AccruedAmount valueObject)
    {
        return valueObject.Value;
    }

    public override string ToString()
    {
        return Value.ToString(CultureInfo.InvariantCulture);
    }
}
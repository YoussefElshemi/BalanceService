using System.Globalization;

namespace Core.ValueObjects;

public readonly record struct InterestRate
{
    public InterestRate(decimal value)
    {
        Value = value;
    }

    private decimal Value { get; }

    public static implicit operator decimal(InterestRate valueObject)
    {
        return valueObject.Value;
    }

    public override string ToString()
    {
        return Value.ToString(CultureInfo.InvariantCulture);
    }
}
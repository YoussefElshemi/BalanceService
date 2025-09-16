using System.Globalization;

namespace Core.ValueObjects;

public readonly record struct AccrualBasis
{
    public AccrualBasis(int value)
    {
        Value = value;
    }

    private int Value { get; }

    public static implicit operator int(AccrualBasis valueObject)
    {
        return valueObject.Value;
    }

    public override string ToString()
    {
        return Value.ToString(CultureInfo.InvariantCulture);
    }
}
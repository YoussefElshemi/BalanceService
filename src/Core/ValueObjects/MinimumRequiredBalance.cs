using System.Globalization;

namespace Core.ValueObjects;

public readonly record struct MinimumRequiredBalance
{
    public MinimumRequiredBalance(decimal value)
    {
        Value = value;
    }

    private decimal Value { get; }

    public static implicit operator decimal(MinimumRequiredBalance valueObject)
    {
        return valueObject.Value;
    }

    public override string ToString()
    {
        return Value.ToString(CultureInfo.InvariantCulture);
    }
}
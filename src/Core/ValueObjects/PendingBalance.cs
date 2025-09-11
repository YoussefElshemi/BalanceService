using System.Globalization;

namespace Core.ValueObjects;

public readonly record struct PendingBalance
{
    public PendingBalance(decimal value)
    {
        Value = value;
    }

    private decimal Value { get; }

    public static implicit operator decimal(PendingBalance valueObject)
    {
        return valueObject.Value;
    }

    public override string ToString()
    {
        return Value.ToString(CultureInfo.InvariantCulture);
    }
}
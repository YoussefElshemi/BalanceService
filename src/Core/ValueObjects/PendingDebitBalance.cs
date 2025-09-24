using System.Globalization;

namespace Core.ValueObjects;

public readonly record struct PendingDebitBalance
{
    public PendingDebitBalance(decimal value)
    {
        Value = value;
    }

    private decimal Value { get; }

    public static implicit operator decimal(PendingDebitBalance valueObject)
    {
        return valueObject.Value;
    }

    public override string ToString()
    {
        return Value.ToString(CultureInfo.InvariantCulture);
    }
}
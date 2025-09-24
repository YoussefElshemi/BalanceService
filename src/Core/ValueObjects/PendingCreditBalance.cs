using System.Globalization;

namespace Core.ValueObjects;

public readonly record struct PendingCreditBalance
{
    public PendingCreditBalance(decimal value)
    {
        Value = value;
    }

    private decimal Value { get; }

    public static implicit operator decimal(PendingCreditBalance valueObject)
    {
        return valueObject.Value;
    }

    public override string ToString()
    {
        return Value.ToString(CultureInfo.InvariantCulture);
    }
}
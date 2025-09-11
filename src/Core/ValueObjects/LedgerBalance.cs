using System.Globalization;

namespace Core.ValueObjects;

public readonly record struct LedgerBalance
{
    public LedgerBalance(decimal value)
    {
        Value = value;
    }

    private decimal Value { get; }

    public static implicit operator decimal(LedgerBalance valueObject)
    {
        return valueObject.Value;
    }

    public override string ToString()
    {
        return Value.ToString(CultureInfo.InvariantCulture);
    }
}
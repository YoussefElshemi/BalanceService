using System.Globalization;

namespace Core.ValueObjects;

public readonly record struct TransferAmount
{
    public TransferAmount(decimal value)
    {
        Value = value;
    }

    private decimal Value { get; }

    public static implicit operator decimal(TransferAmount valueObject)
    {
        return valueObject.Value;
    }

    public override string ToString()
    {
        return Value.ToString(CultureInfo.InvariantCulture);
    }
}
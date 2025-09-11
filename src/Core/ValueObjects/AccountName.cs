namespace Core.ValueObjects;

public readonly record struct AccountName
{
    public AccountName(string value)
    {
        Value = value;
    }

    private string Value { get; }

    public static implicit operator string(AccountName valueObject)
    {
        return valueObject.Value;
    }

    public override string ToString()
    {
        return Value;
    }
}
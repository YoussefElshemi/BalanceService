namespace Core.ValueObjects;

public readonly record struct AccountId
{
    public AccountId(Guid value)
    {
        Value = value;
    }

    private Guid Value { get; }

    public static implicit operator Guid(AccountId valueObject)
    {
        return valueObject.Value;
    }

    public override string ToString()
    {
        return Value.ToString();
    }
}
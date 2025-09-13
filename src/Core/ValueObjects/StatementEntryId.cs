namespace Core.ValueObjects;

public readonly record struct StatementEntryId
{
    public StatementEntryId(Guid value)
    {
        Value = value;
    }

    private Guid Value { get; }

    public static implicit operator Guid(StatementEntryId valueObject)
    {
        return valueObject.Value;
    }

    public override string ToString()
    {
        return Value.ToString();
    }
}
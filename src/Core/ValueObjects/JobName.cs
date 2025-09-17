namespace Core.ValueObjects;

public readonly record struct JobName
{
    public JobName(string value)
    {
        Value = value;
    }

    private string Value { get; }

    public static implicit operator string(JobName valueObject)
    {
        return valueObject.Value;
    }

    public override string ToString()
    {
        return Value;
    }
}
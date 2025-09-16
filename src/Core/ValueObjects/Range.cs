namespace Core.ValueObjects;

public readonly record struct Range<T>(T From, T To)
{
    public override string ToString()
    {
        return $"{From} - {To}";
    }
}
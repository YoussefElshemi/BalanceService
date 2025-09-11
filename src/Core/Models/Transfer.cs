namespace Core.Models;

public record Transfer
{
    public required Transaction DebitTransaction { get; init; }
    public required Transaction CreditTransaction { get; init; }
}
using Core.ValueObjects;

namespace Core.Models;

public record BalanceRequest
{
    public required AccountId AccountId { get; init; }
    public required DateTimeOffset DateTime { get; init; }
}
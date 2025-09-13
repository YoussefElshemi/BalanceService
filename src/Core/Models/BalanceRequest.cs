using Core.ValueObjects;

namespace Core.Models;

public record BalanceRequest
{
    public required AccountId AccountId { get; init; }
    public required DateOnly Date { get; init; }
}
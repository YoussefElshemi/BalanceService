using System.Text.Json;
using Core.Enums;
using Core.ValueObjects;

namespace Core.Models;

public record UpdateAccountRequest
{
    public required AccountName AccountName { get; init; }
    public required CurrencyCode CurrencyCode { get; init; }
    public required MinimumRequiredBalance MinimumRequiredBalance { get; init; }
    public required AccountType AccountType { get; init; }
    public required JsonDocument? Metadata { get; init; }
    public required AccountId? ParentAccountId { get; init; }
    public required Username UpdatedBy { get; init; }
}
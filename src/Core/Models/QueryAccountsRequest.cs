using System.Text.Json;
using Core.Enums;
using Core.ValueObjects;

namespace Core.Models;

public record QueryAccountsRequest
{
    public required PageSize PageSize { get; init; }
    public required PageNumber PageNumber { get; init; }
    public required AccountName? AccountName { get; init; }
    public required CurrencyCode? CurrencyCode { get; init; }
    public required AccountType? AccountType { get; init; }
    public required AccountId? ParentAccountId { get; init; }
    public required AccountName? ParentAccountName { get; init; }
    public required JsonDocument? Metadata { get; init; }
}
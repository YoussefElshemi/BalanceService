using System.ComponentModel;
using Core.Enums;

namespace Presentation.Models;

public record QueryAccountsRequestDto
{
    [DefaultValue(20)]
    public required int PageSize { get; init; } = 20;
    [DefaultValue(1)]
    public required int PageNumber { get; init; } = 1;
    public string? AccountName { get; init; }
    public CurrencyCode? CurrencyCode { get; init; }
    public AccountType? AccountType { get; init; }
    public Guid? ParentAccountId { get; init; }
    public string? ParentAccountName { get; init; }
    public string? Metadata { get; init; }
}

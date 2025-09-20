using System.ComponentModel;
using Core.Enums;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Models.Accounts;

public record QueryAccountsRequestDto : BaseReadRequestDto
{
    [DefaultValue(20)]
    [FromQuery]
    public required int PageSize { get; init; } = 20;

    [DefaultValue(1)]
    [FromQuery]
    public required int PageNumber { get; init; } = 1;
    
    [FromQuery]
    public string? AccountName { get; init; }
    
    [FromQuery]
    public CurrencyCode? CurrencyCode { get; init; }
    
    [FromQuery]
    public AccountType? AccountType { get; init; }
    
    [FromQuery]
    public Guid? ParentAccountId { get; init; }
    
    [FromQuery]
    public string? ParentAccountName { get; init; }
    
    [FromQuery]
    public string? Metadata { get; init; }
}

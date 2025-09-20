using System.Text.Json;
using Core.Enums;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Models.Accounts;

public record UpdateAccountRequestDto : BaseWriteRequestDto
{
    [FromRoute]
    public required Guid AccountId { get; init; }

    [FromBody]
    public required string AccountName { get; init; }
    
    [FromBody]
    public required CurrencyCode CurrencyCode { get; init; }

    [FromBody]
    public decimal? MinimumRequiredBalance { get; init; }
    
    [FromBody]
    public required AccountType AccountType { get; init; }
    
    [FromBody]
    public JsonDocument? Metadata { get; init; }
    
    [FromBody]
    public Guid? ParentAccountId { get; init; }
}

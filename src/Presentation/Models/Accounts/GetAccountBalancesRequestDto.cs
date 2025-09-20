using Microsoft.AspNetCore.Mvc;

namespace Presentation.Models.Accounts;

public record GetAccountBalancesRequestDto : BaseReadRequestDto
{
    [FromRoute]
    public required Guid AccountId { get; init; }
}
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Models.Accounts;

public record GetAccountRequestDto : BaseReadRequestDto
{
    [FromRoute]
    public required Guid AccountId { get; init; }
}
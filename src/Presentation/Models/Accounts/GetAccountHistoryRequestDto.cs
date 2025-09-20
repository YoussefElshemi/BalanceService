using Microsoft.AspNetCore.Mvc;

namespace Presentation.Models.Accounts;

public record GetAccountHistoryRequestDto : GetHistoryRequestDto
{
    [FromRoute]
    public required Guid AccountId { get; init; }
}
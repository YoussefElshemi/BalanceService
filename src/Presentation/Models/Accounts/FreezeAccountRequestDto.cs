using Microsoft.AspNetCore.Mvc;

namespace Presentation.Models.Accounts;

public record FreezeAccountRequestDto : BaseWriteRequestDto
{
    [FromRoute]
    public required Guid AccountId { get; init; }
}
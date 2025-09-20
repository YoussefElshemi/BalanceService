using Microsoft.AspNetCore.Mvc;

namespace Presentation.Models.Accounts;

public record CloseAccountRequestDto : BaseWriteRequestDto
{
    [FromRoute]
    public required Guid AccountId { get; init; }
}
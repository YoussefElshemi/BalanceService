using Microsoft.AspNetCore.Mvc;

namespace Presentation.Models.Accounts;

public record DeleteAccountRequestDto : BaseWriteRequestDto
{
    [FromRoute]
    public required Guid AccountId { get; init; }
}
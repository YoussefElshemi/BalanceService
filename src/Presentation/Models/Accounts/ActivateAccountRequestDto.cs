using Microsoft.AspNetCore.Mvc;

namespace Presentation.Models.Accounts;

public record ActivateAccountRequestDto : BaseWriteRequestDto
{
    [FromRoute]
    public required Guid AccountId { get; init; }
}
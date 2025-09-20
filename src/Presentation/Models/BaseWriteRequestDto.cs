using Microsoft.AspNetCore.Mvc;
using Presentation.Constants;

namespace Presentation.Models;

public record BaseWriteRequestDto : BaseReadRequestDto
{
    [FromHeader(Name = HeaderNames.Username)]
    public required string Username { get; init; }
}
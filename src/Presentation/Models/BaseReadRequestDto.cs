using Microsoft.AspNetCore.Mvc;
using Presentation.Constants;

namespace Presentation.Models;

public record BaseReadRequestDto
{
    [FromHeader(Name = HeaderNames.CorrelationId)]
    public Guid? CorrelationId { get; init; }
}
using System.Text.Json;
using Core.Enums;

namespace Presentation.Models;

public record CreateInterestProductAccountLinkRequestDto
{
    public required Guid InterestProductId { get; init; }
    public DateTimeOffset? ExpiresAt { get; init; }
}
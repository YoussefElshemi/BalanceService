using Core.ValueObjects;

namespace Core.Models;

public record QueryInterestProductAccountLinksRequest
{
    public required PageSize PageSize { get; init; }
    public required PageNumber PageNumber { get; init; }
    public required bool? IsActive { get; init; }
}
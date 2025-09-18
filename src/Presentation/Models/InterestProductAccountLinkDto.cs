namespace Presentation.Models;

public record InterestProductAccountLinkDto
{
    public required Guid AccountId { get; init; }
    public required InterestProductDto InterestProduct { get; init; }
    public required bool IsActive { get; init; }
    public required DateTimeOffset CreatedAt { get; init; }
    public required string CreatedBy { get; init; }
    public required DateTimeOffset UpdatedAt { get; init; }
    public required string UpdatedBy { get; init; }
    public required bool IsDeleted { get; init; }
    public required DateTimeOffset? DeletedAt { get; init; }
    public required string? DeletedBy { get; init; }
}
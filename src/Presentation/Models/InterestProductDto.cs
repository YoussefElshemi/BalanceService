using Core.Enums;

namespace Presentation.Models;

public record InterestProductDto
{
    public required Guid InterestProductId { get; init; }
    public required string Name { get; init; }
    public required decimal AnnualInterestRate { get; init; }
    public required InterestPayoutFrequency InterestPayoutFrequency { get; init; }
    public required decimal AccrualBasis { get; init; }
    public required DateTimeOffset CreatedAt { get; init; }
    public required string CreatedBy { get; init; }
    public required DateTimeOffset UpdatedAt { get; init; }
    public required string UpdatedBy { get; init; }
    public required bool IsDeleted { get; init; }
    public required DateTimeOffset? DeletedAt { get; init; }
    public required string? DeletedBy { get; init; }
}
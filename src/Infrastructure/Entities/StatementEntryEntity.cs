namespace Infrastructure.Entities;

public record StatementEntryEntity
{
    public required Guid StatementEntryId { get; init; }
    public required Guid AccountId { get; init; }
    public required DateTimeOffset ActionedAt { get; init; }
    public required decimal AvailableBalance  { get; init; }
    public required decimal Amount { get; init; }
    public required string CurrencyCode { get; init; }
    public required int StatementTypeId { get; init; }
    public required int StatementDirectionId { get; init; }
    public required string? Description { get; init; }
    public required string? Reference { get; init; }
}
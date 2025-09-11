namespace Infrastructure.Entities;

public record DeletableBaseEntity : BaseEntity
{
    public required bool IsDeleted { get; set; }
    public required DateTimeOffset? DeletedAt { get; set; }
    public required string? DeletedBy { get; set; }
}
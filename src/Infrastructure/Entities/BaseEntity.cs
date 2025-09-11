namespace Infrastructure.Entities;

public record BaseEntity
{
    public required DateTimeOffset CreatedAt { get; init; }
    public required string CreatedBy { get; init; }

    public required DateTimeOffset UpdatedAt { get; set; }
    public required string UpdatedBy { get; set; }

    public uint RowVersion { get; init; }
}
using Core.ValueObjects;

namespace Core.Models;

public record BaseModel
{
    public required CreatedAt CreatedAt { get; init; }
    public required Username CreatedBy { get; init; }
    public required UpdatedAt UpdatedAt { get; init; }
    public required Username UpdatedBy { get; init; }
}
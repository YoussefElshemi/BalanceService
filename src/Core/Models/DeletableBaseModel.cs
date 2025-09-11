using Core.ValueObjects;

namespace Core.Models;

public record DeletableBaseModel : BaseModel
{
    public required bool IsDeleted { get; init; }
    public required DeletedAt? DeletedAt { get; init; }
    public required Username? DeletedBy { get; init; }
}
using Core.ValueObjects;

namespace Core.Models;

public record Job : DeletableBaseModel
{
    public required JobId JobId { get; init; }
    public required JobName JobName { get; init; }
}
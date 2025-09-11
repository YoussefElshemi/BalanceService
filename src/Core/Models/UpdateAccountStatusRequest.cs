using Core.Enums;
using Core.ValueObjects;

namespace Core.Models;

public record UpdateAccountStatusRequest
{
    public required AccountId AccountId { get; init; }
    public required AccountStatus AccountStatus { get; init; }
    public required Username UpdatedBy { get; init; }
}
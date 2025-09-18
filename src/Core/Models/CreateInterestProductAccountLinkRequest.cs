using System.Text.Json;
using Core.Enums;
using Core.ValueObjects;

namespace Core.Models;

public record CreateInterestProductAccountLinkRequest
{
    public required AccountId AccountId { get; init; }
    public required InterestProductId InterestProductId { get; init; }
    public required Username CreatedBy { get; init; }
}
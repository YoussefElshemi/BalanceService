using System.ComponentModel;
using Core.Enums;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Models.Holds;

public record QueryHoldsRequestDto
{
    [DefaultValue(20)]
    [FromQuery]
    public required int PageSize { get; init; } = 20;

    [DefaultValue(1)]
    [FromQuery]
    public required int PageNumber { get; init; } = 1;
    
    [FromQuery]
    public Guid? AccountId { get; init; }
    
    [FromQuery]
    public CurrencyCode? CurrencyCode { get; init; }
    
    [FromQuery]
    public decimal? Amount { get; init; }
    
    [FromQuery]
    public Guid? SettledTransactionId { get; init; }
    
    [FromQuery]
    public DateTimeOffset? ExpiresAt { get; init; }
    
    [FromQuery]
    public DateTimeOffset? FromCreatedAt { get; init; }
    
    [FromQuery]
    public DateTimeOffset? ToCreatedAt { get; init; }
    
    [FromQuery]
    public HoldType? Type { get; init; }
    
    [FromQuery]
    public HoldStatus? Status { get; init; }
    
    [FromQuery]
    public HoldSource? Source { get; init; }
    
    [FromQuery]
    public string? Description { get; init; }
    
    [FromQuery]
    public string? Reference { get; init; }
}

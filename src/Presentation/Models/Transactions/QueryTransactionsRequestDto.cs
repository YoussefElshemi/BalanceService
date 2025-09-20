using System.ComponentModel;
using Core.Enums;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Models.Transactions;

public record QueryTransactionsRequestDto : BaseReadRequestDto
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
    public DateOnly? FromPostedDate { get; init; }
    
    [FromQuery]
    public DateOnly? ToPostedDate { get; init; }
    
    [FromQuery]
    public TransactionDirection? Direction { get; init; }
    
    [FromQuery]
    public TransactionType? Type { get; init; }
    
    [FromQuery]
    public TransactionStatus? Status { get; init; }
    
    [FromQuery]
    public TransactionSource? Source { get; init; }
    
    [FromQuery]
    public string? Description { get; init; }
    
    [FromQuery]
    public string? Reference { get; init; }
}

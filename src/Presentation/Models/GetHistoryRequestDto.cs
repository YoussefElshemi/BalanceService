using System.ComponentModel;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Models;

public record GetHistoryRequestDto : BaseReadRequestDto
{
    [DefaultValue(20)]
    [FromQuery]
    public required int PageSize { get; init; } = 20;

    [DefaultValue(1)]
    [FromQuery]
    public required int PageNumber { get; init; } = 1;
    
    [FromQuery]
    public bool? IgnoreInsert { get; init; }
}
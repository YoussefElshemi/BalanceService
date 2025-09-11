using System.ComponentModel.DataAnnotations;
using Core.Interfaces;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Presentation.Constants;
using Presentation.Mappers;
using Presentation.Models;

namespace Presentation.Controllers;

/// <summary>
/// Manages operations on transfers
/// </summary>
[ApiController]
[Route("transfers")]
[Produces("application/json")]
[ProducesResponseType(StatusCodes.Status400BadRequest)]
[ProducesResponseType(StatusCodes.Status500InternalServerError)]
public class TransfersController : Controller
{
    /// <summary>
    /// Creates a transfer.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(TransferDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> CreateTransfer(
        [FromBody] CreateTransferRequestDto createTransferRequestDto,
        [FromHeader(Name = HeaderNames.CorrelationId)] Guid correlationId,
        [Required] [FromHeader(Name = HeaderNames.Username)] string username,
        [FromServices] IValidator<CreateTransferRequestDto> createTransferRequestDtoValidator,
        [FromServices] ITransferService transferService,
        CancellationToken cancellationToken)
    {
        await createTransferRequestDtoValidator.ValidateAndThrowAsync(createTransferRequestDto, cancellationToken);

        var createTransferRequest = createTransferRequestDto.ToModel(username);

        var transfer = await transferService.CreateAsync(createTransferRequest, cancellationToken);

        return Created(string.Empty, transfer.ToDto());
    }
}
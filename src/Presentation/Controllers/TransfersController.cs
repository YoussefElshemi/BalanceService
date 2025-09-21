using System.Net.Mime;
using Core.Interfaces;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Presentation.CustomBinding;
using Presentation.Mappers.Transfers;
using Presentation.Models.Transfers;

namespace Presentation.Controllers;

/// <summary>
/// Manages operations on transfers
/// </summary>
[ApiController]
[Route("transfers")]
[Produces(MediaTypeNames.Application.Json)]
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
        [FromHybrid] CreateTransferRequestDto createTransferRequestDto,
        [FromServices] IValidator<CreateTransferRequestDto> createTransferRequestDtoValidator,
        [FromServices] ITransferService transferService,
        CancellationToken cancellationToken)
    {
        await createTransferRequestDtoValidator.ValidateAndThrowAsync(createTransferRequestDto, cancellationToken);

        var createTransferRequest = createTransferRequestDto.ToModel();

        var transfer = await transferService.CreateAsync(createTransferRequest, cancellationToken);

        return Created(string.Empty, transfer.ToDto());
    }
}
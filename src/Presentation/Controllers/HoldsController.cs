using System.Net.Mime;
using Core.Interfaces;
using Core.ValueObjects;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Presentation.CustomBinding;
using Presentation.Mappers;
using Presentation.Mappers.Holds;
using Presentation.Mappers.Transactions;
using Presentation.Models;
using Presentation.Models.Holds;
using Presentation.Models.Transactions;

namespace Presentation.Controllers;

/// <summary>
/// Manages operations on holds
/// </summary>
[ApiController]
[Route("holds")]
[Produces(MediaTypeNames.Application.Json)]
[ProducesResponseType(StatusCodes.Status400BadRequest)]
[ProducesResponseType(StatusCodes.Status500InternalServerError)]
public class HoldsController : Controller
{
    /// <summary>
    /// Creates an active hold.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(HoldDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> CreateHold(
        [FromHybrid] CreateHoldRequestDto createHoldRequestDto,
        [FromServices] IValidator<CreateHoldRequestDto> createHoldRequestDtoValidator,
        [FromServices] IHoldService holdService,
        CancellationToken cancellationToken)
    {
        await createHoldRequestDtoValidator.ValidateAndThrowAsync(createHoldRequestDto, cancellationToken);

        var createHoldRequest = createHoldRequestDto.ToModel();

        var hold = await holdService.CreateAsync(createHoldRequest, cancellationToken);

        return CreatedAtAction(
            actionName: nameof(GetHoldById),
            controllerName: "Holds", 
            routeValues: new { hold.HoldId },
            value: hold.ToDto()
        );
    }

    /// <summary>
    /// Queries holds with optional filters and pagination.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(PagedResultsDto<HoldDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> QueryHolds(
        [FromHybrid] QueryHoldsRequestDto queryHoldsRequestDto,
        [FromServices] IValidator<QueryHoldsRequestDto> queryHoldsRequestDtoValidator,
        [FromServices] IHoldService holdService,
        CancellationToken cancellationToken)
    {
        await queryHoldsRequestDtoValidator.ValidateAndThrowAsync(queryHoldsRequestDto, cancellationToken);
    
        var queryHoldsRequest = queryHoldsRequestDto.ToModel();
    
        var results = await holdService.QueryAsync(queryHoldsRequest, cancellationToken);
    
        return Ok(results.ToDto(x => x.ToDto()));
    }
    
    /// <summary>
    /// Gets a hold by its ID.
    /// </summary>
    [HttpGet("{holdId:guid}")]
    [ProducesResponseType(typeof(HoldDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetHoldById(
        [FromHybrid] GetHoldRequestDto getHoldRequestDto,
        [FromServices] IHoldService holdService,
        CancellationToken cancellationToken)
    {
        var hold = await holdService.GetByIdAsync(new HoldId(getHoldRequestDto.HoldId), cancellationToken);
    
        return Ok(hold.ToDto());
    }
    
    /// <summary>
    /// Releases an active hold by its ID.
    /// </summary>
    [HttpPost("{holdId:guid}/release")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> ReleaseHoldById(
        [FromBody] ReleaseHoldRequestDto releaseHoldRequestDto,
        [FromServices] IHoldService holdService,
        CancellationToken cancellationToken)
    {
        await holdService.ReleaseAsync(
            new HoldId(releaseHoldRequestDto.HoldId),
            new Username(releaseHoldRequestDto.Username),
            cancellationToken);

        return NoContent();
    }
    
    /// <summary>
    /// Settles an active hold by its ID.
    /// </summary>
    [HttpPost("{holdId:guid}/settle")]
    [ProducesResponseType(typeof(TransactionDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> SettleHoldById(
        [FromHybrid] SettleHoldRequestDto settleHoldRequestDto,
        [FromServices] IHoldService holdService,
        CancellationToken cancellationToken)
    {
        var transaction = await holdService.SettleAsync(
            new HoldId(settleHoldRequestDto.HoldId),
            new Username(settleHoldRequestDto.Username),
            cancellationToken);

        return CreatedAtAction(
            actionName: nameof(TransactionsController.GetTransactionById),
            controllerName: "Transactions", 
            routeValues: new { transaction.TransactionId },
            value: transaction.ToDto()
        );
    }

    /// <summary>|
    /// Gets a hold's history by its ID.
    /// </summary>
    [HttpGet("{holdId:guid}/history")]
    [ProducesResponseType(typeof(PagedResultsDto<ChangeEventDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetHoldHistoryById(
        [FromHybrid] GetHoldHistoryRequestDto getHoldHistoryRequestDto,
        [FromServices] IValidator<GetHistoryRequestDto> getHistoryRequestDtoValidator,
        [FromServices] IHoldService holdService,
        CancellationToken cancellationToken)
    {
        await getHistoryRequestDtoValidator.ValidateAndThrowAsync(getHoldHistoryRequestDto, cancellationToken);

        var getHistoryRequest = getHoldHistoryRequestDto.ToModel(getHoldHistoryRequestDto.HoldId);

        var results = await holdService.GetHistoryAsync(getHistoryRequest, cancellationToken);

        return Ok(results.ToDto(x => x.ToDto()));
        
    }

    /// <summary>
    /// Updates an existing draft hold.
    /// </summary>
    [HttpPut("{holdId:guid}")]
    [ProducesResponseType(typeof(HoldDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> UpdateHold(
        [FromHybrid] UpdateHoldRequestDto updateHoldRequestDto,
        [FromServices] IValidator<UpdateHoldRequestDto> updateHoldRequestDtoValidator,
        [FromServices] IHoldService holdService,
        CancellationToken cancellationToken)
    {
        await updateHoldRequestDtoValidator.ValidateAndThrowAsync(updateHoldRequestDto, cancellationToken);

        var updateHoldRequest = updateHoldRequestDto.ToModel();
    
        var updatedHold = await holdService.UpdateAsync(updateHoldRequest, cancellationToken);
    
        return Ok(updatedHold.ToDto());
    }
    
    /// <summary>
    /// Deletes a hold.
    /// </summary>
    [HttpDelete("{holdId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> DeleteHold(
        [FromHybrid] DeleteHoldRequestDto deleteHoldRequestDto,
        [FromServices] IHoldService holdService,
        CancellationToken cancellationToken)
    {
        await holdService.DeleteAsync(
            new HoldId(deleteHoldRequestDto.HoldId),
            new Username(deleteHoldRequestDto.Username),
            cancellationToken);
    
        return NoContent();
    }
}

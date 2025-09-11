using System.ComponentModel.DataAnnotations;
using Core.Interfaces;
using Core.ValueObjects;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Presentation.Constants;
using Presentation.Mappers;
using Presentation.Models;

namespace Presentation.Controllers;

/// <summary>
/// Manages operations on holds
/// </summary>
[ApiController]
[Route("holds")]
[Produces("application/json")]
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
        [FromBody] CreateHoldRequestDto createHoldRequestDto,
        [FromHeader(Name = HeaderNames.CorrelationId)] Guid correlationId,
        [Required][FromHeader(Name = HeaderNames.Username)] string username,
        [FromServices] IValidator<CreateHoldRequestDto> createHoldRequestDtoValidator,
        [FromServices] IHoldService holdService,
        CancellationToken cancellationToken)
    {
        await createHoldRequestDtoValidator.ValidateAndThrowAsync(createHoldRequestDto, cancellationToken);

        var createHoldRequest = createHoldRequestDto.ToModel(username);

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
        [FromQuery] QueryHoldsRequestDto queryHoldsRequestDto,
        [FromHeader(Name = HeaderNames.CorrelationId)] Guid correlationId,
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
        [FromRoute] Guid holdId,
        [FromHeader(Name = HeaderNames.CorrelationId)] Guid correlationId,
        [FromServices] IHoldService holdService,
        CancellationToken cancellationToken)
    {
        var hold = await holdService.GetByIdAsync(new HoldId(holdId), cancellationToken);
    
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
        [FromRoute] Guid holdId,
        [Required][FromHeader(Name = HeaderNames.Username)] string username,
        [FromHeader(Name = HeaderNames.CorrelationId)] Guid correlationId,
        [FromServices] IHoldService holdService,
        CancellationToken cancellationToken)
    {
        await holdService.ReleaseAsync(new HoldId(holdId), new Username(username), cancellationToken);

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
        [FromRoute] Guid holdId,
        [Required][FromHeader(Name = HeaderNames.Username)] string username,
        [FromHeader(Name = HeaderNames.CorrelationId)] Guid correlationId,
        [FromServices] IHoldService holdService,
        CancellationToken cancellationToken)
    {
        var transaction = await holdService.SettleAsync(new HoldId(holdId), new Username(username), cancellationToken);

        return CreatedAtAction(
            actionName: nameof(TransactionsController.GetTransactionById),
            controllerName: "Transactions", 
            routeValues: new { transaction.TransactionId },
            value: transaction.ToDto()
        );
    }
    
    /// <summary>
    /// Updates an existing draft hold.
    /// </summary>
    [HttpPut("{holdId:guid}")]
    [ProducesResponseType(typeof(HoldDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> UpdateHold(
        [FromRoute] Guid holdId,
        [FromBody] UpdateHoldRequestDto updateHoldRequestDto,
        [FromHeader(Name = HeaderNames.CorrelationId)] Guid correlationId,
        [Required][FromHeader(Name = HeaderNames.Username)] string username,
        [FromServices] IValidator<UpdateHoldRequestDto> updateHoldRequestDtoValidator,
        [FromServices] IHoldService holdService,
        CancellationToken cancellationToken)
    {
        await updateHoldRequestDtoValidator.ValidateAndThrowAsync(updateHoldRequestDto, cancellationToken);

        var updateHoldRequest = updateHoldRequestDto.ToModel(username);
    
        var updatedHold = await holdService.UpdateAsync(new HoldId(holdId), updateHoldRequest, cancellationToken);
    
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
        [FromRoute] Guid holdId,
        [FromHeader(Name = HeaderNames.CorrelationId)] Guid correlationId,
        [Required][FromHeader(Name = HeaderNames.Username)] string username,
        [FromServices] IHoldService holdService,
        CancellationToken cancellationToken)
    {
        await holdService.DeleteAsync(new HoldId(holdId), new Username(username), cancellationToken);
    
        return NoContent();
    }
}

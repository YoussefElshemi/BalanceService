using System.ComponentModel.DataAnnotations;
using System.Net.Mime;
using Core.Interfaces;
using Core.ValueObjects;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Presentation.Constants;
using Presentation.Mappers;
using Presentation.Models;

namespace Presentation.Controllers;

/// <summary>
/// Manages operations on accounts
/// </summary>
[ApiController]
[Route("accounts/{accountId:guid}/interest")]
[Produces(MediaTypeNames.Application.Json)]
[ProducesResponseType(StatusCodes.Status400BadRequest)]
[ProducesResponseType(StatusCodes.Status500InternalServerError)]
public class InterestProductAccountLinksController : Controller
{
    /// <summary>
    /// Creates a new interest product account link
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(InterestProductAccountLinkDto), StatusCodes.Status201Created)]
    public async Task<IActionResult> CreateInterestProductAccountLink(
        [FromRoute] Guid accountId,
        [FromBody] CreateInterestProductAccountLinkRequestDto createInterestProductAccountLinkRequestDto,
        [FromHeader(Name = HeaderNames.CorrelationId)] Guid correlationId,
        [Required][FromHeader(Name = HeaderNames.Username)] string username,
        [FromServices] IValidator<CreateInterestProductAccountLinkRequestDto> createInterestProductAccountLinkRequestDtoValidator,
        [FromServices] IInterestProductAccountLinkService interestProductAccountLinkService,
        CancellationToken cancellationToken)
    {
        await createInterestProductAccountLinkRequestDtoValidator.ValidateAndThrowAsync(createInterestProductAccountLinkRequestDto, cancellationToken);

        var createInterestProductAccountLinkRequest = createInterestProductAccountLinkRequestDto.ToModel(accountId, username);

        var interestProductAccountLink = await interestProductAccountLinkService.CreateAsync(createInterestProductAccountLinkRequest, cancellationToken);

        return CreatedAtAction(
            actionName: nameof(GetInterestProductAccountLinkById),
            controllerName: "InterestProductAccountLinks", 
            routeValues: new { interestProductAccountLink.Account.AccountId, interestProductAccountLink.InterestProduct.InterestProductId },
            value: interestProductAccountLink.ToDto()
        );
    }
    
    /// <summary>
    /// Queries interest product account links with optional filters and pagination.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(PagedResultsDto<InterestProductAccountLinkDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> QueryInterestProductAccountLinks(
        [FromQuery] QueryInterestProductAccountLinksRequestDto queryInterestProductAccountLinksRequestDto,
        [FromHeader(Name = HeaderNames.CorrelationId)] Guid correlationId,
        [FromServices] IValidator<QueryInterestProductAccountLinksRequestDto> queryInterestProductAccountLinksRequestDtoValidator,
        [FromServices] IInterestProductAccountLinkService interestProductAccountLinkServiceService,
        CancellationToken cancellationToken)
    {
        await queryInterestProductAccountLinksRequestDtoValidator.ValidateAndThrowAsync(queryInterestProductAccountLinksRequestDto, cancellationToken);
    
        var queryInterestProductAccountLinksRequest = queryInterestProductAccountLinksRequestDto.ToModel();
    
        var results = await interestProductAccountLinkServiceService.QueryAsync(queryInterestProductAccountLinksRequest, cancellationToken);
    
        return Ok(results.ToDto(x => x.ToDto()));
    }
    
    /// <summary>
    /// Gets an interest product account link by its ID.
    /// </summary>
    [HttpGet("{interestProductId:guid}")]
    [ProducesResponseType(typeof(InterestProductAccountLinkDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetInterestProductAccountLinkById(
        [FromRoute] Guid accountId,
        [FromRoute] Guid interestProductId,
        [FromHeader(Name = HeaderNames.CorrelationId)] Guid correlationId,
        [FromServices] IInterestProductAccountLinkService interestProductAccountLinkService,
        CancellationToken cancellationToken)
    {
        var interestProductAccountLink = await interestProductAccountLinkService.GetByIdAsync(
            new AccountId(accountId),
            new InterestProductId(interestProductId),
            cancellationToken);
    
        return Ok(interestProductAccountLink.ToDto());
    }
    
    /// <summary>
    /// Activates an interest product account link.
    /// </summary>
    [HttpPost("{interestProductId:guid}/activate")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ActivateAccountById(
        [FromRoute] Guid accountId,
        [FromRoute] Guid interestProductId,
        [FromHeader(Name = HeaderNames.CorrelationId)] Guid correlationId,
        [Required][FromHeader(Name = HeaderNames.Username)] string username,
        [FromServices] IInterestProductAccountLinkService interestProductAccountLinkService,
        CancellationToken cancellationToken)
    {
        await interestProductAccountLinkService.ActivateAsync(
            new AccountId(accountId),
            new InterestProductId(interestProductId),
            new Username(username),
            cancellationToken);
    
        return NoContent();
    }
    
    /// <summary>
    /// Deactivates an interest product account link.
    /// </summary>
    [HttpPost("{interestProductId:guid}/deactivate")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeactivateAccountById(
        [FromRoute] Guid accountId,
        [FromRoute] Guid interestProductId,
        [FromHeader(Name = HeaderNames.CorrelationId)] Guid correlationId,
        [Required][FromHeader(Name = HeaderNames.Username)] string username,
        [FromServices] IInterestProductAccountLinkService interestProductAccountLinkService,
        CancellationToken cancellationToken)
    {
        await interestProductAccountLinkService.DeactivateAsync(
            new AccountId(accountId),
            new InterestProductId(interestProductId),
            new Username(username),
            cancellationToken);
    
        return NoContent();
    }

    /// <summary>
    /// Updates an existing interest product.
    /// </summary>
    [HttpPut("{interestProductId:guid}")]
    [ProducesResponseType(typeof(InterestProductAccountLinkDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> UpdateInterestProduct(
        [FromRoute] Guid accountId,
        [FromRoute] Guid interestProductId,
        [FromBody] UpdateInterestProductAccountLinkRequestDto updateInterestProductAccountLinkRequestDto,
        [FromHeader(Name = HeaderNames.CorrelationId)] Guid correlationId,
        [Required][FromHeader(Name = HeaderNames.Username)] string username,
        [FromServices] IValidator<UpdateInterestProductAccountLinkRequestDto> updateInterestProductAccountLinkRequestDtoValidator,
        [FromServices] IInterestProductAccountLinkService interestProductAccountLinkService,
        CancellationToken cancellationToken)
    {
        await updateInterestProductAccountLinkRequestDtoValidator.ValidateAndThrowAsync(updateInterestProductAccountLinkRequestDto, cancellationToken);
    
        var updateInterestProductAccountLinkRequest = updateInterestProductAccountLinkRequestDto.ToModel(
            accountId, 
            interestProductId,
            username);
    
        var interestProductAccountLink = await interestProductAccountLinkService.UpdateAsync(
            updateInterestProductAccountLinkRequest,
            cancellationToken);
    
        return Ok(interestProductAccountLink.ToDto());
    }
    
    /// <summary>
    /// Deletes an interest product.
    /// </summary>
    [HttpDelete("{interestProductId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteInterestProduct(
        [FromRoute] Guid accountId,
        [FromRoute] Guid interestProductId,
        [FromHeader(Name = HeaderNames.CorrelationId)] Guid correlationId,
        [Required][FromHeader(Name = HeaderNames.Username)] string username,
        [FromServices] IInterestProductAccountLinkService interestProductAccountLinkService,
        CancellationToken cancellationToken)
    {
        await interestProductAccountLinkService.DeleteAsync(
            new AccountId(accountId),
            new InterestProductId(interestProductId),
            new Username(username),
            cancellationToken);
    
        return NoContent();
    }
}

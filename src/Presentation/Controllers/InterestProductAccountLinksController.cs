using System.Net.Mime;
using Core.Interfaces;
using Core.ValueObjects;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Presentation.CustomBinding;
using Presentation.Mappers;
using Presentation.Mappers.InterestProductAccountLinks;
using Presentation.Models;
using Presentation.Models.InterestProductAccountLinks;

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
        [FromHybrid] CreateInterestProductAccountLinkRequestDto createInterestProductAccountLinkRequestDto,
        [FromServices] IValidator<CreateInterestProductAccountLinkRequestDto> createInterestProductAccountLinkRequestDtoValidator,
        [FromServices] IInterestProductAccountLinkService interestProductAccountLinkService,
        CancellationToken cancellationToken)
    {
        await createInterestProductAccountLinkRequestDtoValidator.ValidateAndThrowAsync(createInterestProductAccountLinkRequestDto, cancellationToken);

        var createInterestProductAccountLinkRequest = createInterestProductAccountLinkRequestDto.ToModel();

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
        [FromHybrid] QueryInterestProductAccountLinksRequestDto queryInterestProductAccountLinksRequestDto,
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
        [FromHybrid] GetInterestProductAccountLinkRequestDto getInterestProductAccountLinkRequestDto,
        [FromServices] IInterestProductAccountLinkService interestProductAccountLinkService,
        CancellationToken cancellationToken)
    {
        var interestProductAccountLink = await interestProductAccountLinkService.GetByIdAsync(
            new AccountId(getInterestProductAccountLinkRequestDto.AccountId),
            new InterestProductId(getInterestProductAccountLinkRequestDto.InterestProductId),
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
    public async Task<IActionResult> ActivateInterestProductAccountLinkById(
        [FromHybrid] ActivateInterestProductAccountLinkRequestDto activateInterestProductAccountLinkRequestDto,
        [FromServices] IInterestProductAccountLinkService interestProductAccountLinkService,
        CancellationToken cancellationToken)
    {
        await interestProductAccountLinkService.ActivateAsync(
            new AccountId(activateInterestProductAccountLinkRequestDto.AccountId),
            new InterestProductId(activateInterestProductAccountLinkRequestDto.InterestProductId),
            new Username(activateInterestProductAccountLinkRequestDto.Username),
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
    public async Task<IActionResult> DeactivateInterestProductAccountLinkById(
        [FromHybrid] DeactivateInterestProductAccountLinkRequestDto deactivateInterestProductAccountLinkRequestDto,
        [FromServices] IInterestProductAccountLinkService interestProductAccountLinkService,
        CancellationToken cancellationToken)
    {
        await interestProductAccountLinkService.DeactivateAsync(
            new AccountId(deactivateInterestProductAccountLinkRequestDto.AccountId),
            new InterestProductId(deactivateInterestProductAccountLinkRequestDto.InterestProductId),
            new Username(deactivateInterestProductAccountLinkRequestDto.Username),
            cancellationToken);
    
        return NoContent();
    }

    /// <summary>
    /// Updates an existing interest product account link.
    /// </summary>
    [HttpPut("{interestProductId:guid}")]
    [ProducesResponseType(typeof(InterestProductAccountLinkDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> UpdateInterestProductAccountLinkById(
        [FromHybrid] UpdateInterestProductAccountLinkRequestDto updateInterestProductAccountLinkRequestDto,
        [FromServices] IValidator<UpdateInterestProductAccountLinkRequestDto> updateInterestProductAccountLinkRequestDtoValidator,
        [FromServices] IInterestProductAccountLinkService interestProductAccountLinkService,
        CancellationToken cancellationToken)
    {
        await updateInterestProductAccountLinkRequestDtoValidator.ValidateAndThrowAsync(updateInterestProductAccountLinkRequestDto, cancellationToken);

        var updateInterestProductAccountLinkRequest = updateInterestProductAccountLinkRequestDto.ToModel();
    
        var interestProductAccountLink = await interestProductAccountLinkService.UpdateAsync(
            updateInterestProductAccountLinkRequest,
            cancellationToken);
    
        return Ok(interestProductAccountLink.ToDto());
    }
    
    /// <summary>
    /// Deletes an interest product account link.
    /// </summary>
    [HttpDelete("{interestProductId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteInterestProductAccountLinkById(
        [FromHybrid] DeleteInterestProductAccountLinkRequestDto deleteInterestProductAccountLinkRequestDto,
        [FromServices] IInterestProductAccountLinkService interestProductAccountLinkService,
        CancellationToken cancellationToken)
    {
        await interestProductAccountLinkService.DeleteAsync(
            new AccountId(deleteInterestProductAccountLinkRequestDto.AccountId),
            new InterestProductId(deleteInterestProductAccountLinkRequestDto.InterestProductId),
            new Username(deleteInterestProductAccountLinkRequestDto.Username),
            cancellationToken);
    
        return NoContent();
    }
}

using System.Net.Mime;
using Core.Interfaces;
using Core.ValueObjects;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Presentation.CustomBinding;
using Presentation.Mappers;
using Presentation.Mappers.InterestProducts;
using Presentation.Models;
using Presentation.Models.InterestProducts;

namespace Presentation.Controllers;

/// <summary>
/// Manages operations on interest products
/// </summary>
[ApiController]
[Route("interest-products")]
[Produces(MediaTypeNames.Application.Json)]
[ProducesResponseType(StatusCodes.Status400BadRequest)]
[ProducesResponseType(StatusCodes.Status500InternalServerError)]
public class InterestProductsController : Controller
{
    /// <summary>
    /// Creates an interest product.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(InterestProductDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> CreateInterestProduct(
        [FromHybrid] CreateInterestProductRequestDto createInterestProductRequestDto,
        [FromServices] IValidator<CreateInterestProductRequestDto> createInterestProductRequestDtoValidator,
        [FromServices] IInterestProductService interestProductService,
        CancellationToken cancellationToken)
    {
        await createInterestProductRequestDtoValidator.ValidateAndThrowAsync(createInterestProductRequestDto, cancellationToken);

        var createInterestProductRequest = createInterestProductRequestDto.ToModel();

        var interestProduct = await interestProductService.CreateAsync(createInterestProductRequest, cancellationToken);

        return CreatedAtAction(
            actionName: nameof(GetInterestProductById),
            controllerName: "InterestProducts", 
            routeValues: new { interestProduct.InterestProductId },
            value: interestProduct.ToDto()
        );
    }
    
    /// <summary>
    /// Queries interest products with optional filters and pagination.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(PagedResultsDto<InterestProductDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> QueryInterestProducts(
        [FromHybrid] QueryInterestProductsRequestDto queryInterestProductsRequestDto,
        [FromServices] IValidator<QueryInterestProductsRequestDto> queryInterestProductsRequestDtoValidator,
        [FromServices] IInterestProductService interestProductService,
        CancellationToken cancellationToken)
    {
        await queryInterestProductsRequestDtoValidator.ValidateAndThrowAsync(queryInterestProductsRequestDto, cancellationToken);
    
        var queryInterestProductsRequest = queryInterestProductsRequestDto.ToModel();
    
        var results = await interestProductService.QueryAsync(queryInterestProductsRequest, cancellationToken);
    
        return Ok(results.ToDto(x => x.ToDto()));
    }
    
    /// <summary>
    /// Gets an interest product by its ID.
    /// </summary>
    [HttpGet("{interestProductId:guid}")]
    [ProducesResponseType(typeof(InterestProductDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetInterestProductById(
        [FromHybrid] GetInterestProductRequestDto getInterestProductRequestDto,
        [FromServices] IInterestProductService interestProductService,
        CancellationToken cancellationToken)
    {
        var interestProduct = await interestProductService.GetByIdAsync(
            new InterestProductId(getInterestProductRequestDto.InterestProductId),
            cancellationToken);
    
        return Ok(interestProduct.ToDto());
    }
    
    /// <summary>
    /// Updates an existing interest product.
    /// </summary>
    [HttpPut("{interestProductId:guid}")]
    [ProducesResponseType(typeof(InterestProductDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> UpdateInterestProduct(
        [FromHybrid] UpdateInterestProductRequestDto updateInterestProductRequestDto,
        [FromServices] IValidator<UpdateInterestProductRequestDto> updateInterestProductRequestDtoValidator,
        [FromServices] IInterestProductService interestProductService,
        CancellationToken cancellationToken)
    {
        await updateInterestProductRequestDtoValidator.ValidateAndThrowAsync(updateInterestProductRequestDto, cancellationToken);
    
        var updateInterestProductRequest = updateInterestProductRequestDto.ToModel();
    
        var updatedInterestProduct = await interestProductService.UpdateAsync(updateInterestProductRequest, cancellationToken);
    
        return Ok(updatedInterestProduct.ToDto());
    }
    
    /// <summary>
    /// Deletes an interest product.
    /// </summary>
    [HttpDelete("{interestProductId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> DeleteInterestProduct(
        [FromHybrid] DeleteInterestProductRequestDto deleteInterestProductRequestDto,
        [FromServices] IInterestProductService interestProductService,
        CancellationToken cancellationToken)
    {
        await interestProductService.DeleteAsync(
            new InterestProductId(deleteInterestProductRequestDto.InterestProductId),
            new Username(deleteInterestProductRequestDto.Username),
            cancellationToken);
    
        return NoContent();
    }
}

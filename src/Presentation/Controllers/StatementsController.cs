using Core.Interfaces;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Presentation.Constants;
using Presentation.Mappers;
using Presentation.Models;

namespace Presentation.Controllers;

/// <summary>
/// Manages operations on statements
/// </summary>
[ApiController]
[Route("statements")]
[Produces("application/json")]
[ProducesResponseType(StatusCodes.Status400BadRequest)]
[ProducesResponseType(StatusCodes.Status500InternalServerError)]
public class StatementsController : Controller
{
    /// <summary>
    /// Gets a statement.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(StatementDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetStatement(
        [FromQuery] GetStatementRequestDto getStatementRequestDto,
        [FromHeader(Name = HeaderNames.CorrelationId)] Guid correlationId,
        [FromServices] IValidator<GetStatementRequestDto> getStatementRequestDtoValidator,
        [FromServices] IStatementService statementService,
        CancellationToken cancellationToken)
    {
        await getStatementRequestDtoValidator.ValidateAndThrowAsync(getStatementRequestDto, cancellationToken);

        var getStatementRequest = getStatementRequestDto.ToModel();

        var statement = await statementService.GetAsync(getStatementRequest, cancellationToken);

        return Ok(statement.ToDto());
    }
}
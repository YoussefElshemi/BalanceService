using System.Net.Mime;
using Core.Interfaces;
using Core.Models;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Presentation.CustomBinding;
using Presentation.Mappers.Statements;
using Presentation.Models.Statements;

namespace Presentation.Controllers;

/// <summary>
/// Manages operations on statements
/// </summary>
[ApiController]
[Route("statements")]
[Produces(MediaTypeNames.Application.Json)]
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
        [FromHybrid] GetStatementRequestDto getStatementRequestDto,
        [FromServices] IValidator<GetStatementRequestDto> getStatementRequestDtoValidator,
        [FromServices] IStatementService statementService,
        CancellationToken cancellationToken)
    {
        await getStatementRequestDtoValidator.ValidateAndThrowAsync(getStatementRequestDto, cancellationToken);

        var getStatementRequest = getStatementRequestDto.ToModel();

        var statement = await statementService.GetAsync(getStatementRequest, cancellationToken);

        return Ok(statement.ToDto());
    }

    /// <summary>
    /// Generates a PDF statement.
    /// </summary>
    [HttpPost("pdf")]
    [Produces(MediaTypeNames.Application.Pdf)]
    [ProducesResponseType(typeof(StatementDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> GeneratePdfStatement(
        [FromHybrid] GenerateStatementRequestDto generateStatementRequestDto,
        [FromServices] IValidator<GenerateStatementRequestDto> generateStatementRequestDtoValidator,
        [FromServices] IStatementService statementService,
        CancellationToken cancellationToken)
    {
        await generateStatementRequestDtoValidator.ValidateAndThrowAsync(generateStatementRequestDto, cancellationToken);

        var generatedPdfStatementRequest = generateStatementRequestDto.ToModel();

        var pdf = await statementService.GeneratePdfAsync(generatedPdfStatementRequest, cancellationToken);

        return File(pdf, MediaTypeNames.Application.Pdf, $"{nameof(Statement)}_{generateStatementRequestDto.AccountId}.pdf");
    }

    /// <summary>
    /// Generates a CSV statement.
    /// </summary>
    [HttpPost("csv")]
    [Produces(MediaTypeNames.Text.Csv)]
    [ProducesResponseType(typeof(FileContentResult), StatusCodes.Status200OK)]
    public async Task<IActionResult> GenerateCsvStatement(
        [FromBody] GenerateStatementRequestDto generateCsvStatementRequestDto,
        [FromServices] IValidator<GenerateStatementRequestDto> generateCsvStatementRequestDtoValidator,
        [FromServices] IStatementService statementService,
        CancellationToken cancellationToken)
    {
        await generateCsvStatementRequestDtoValidator.ValidateAndThrowAsync(generateCsvStatementRequestDto, cancellationToken);

        var generatedCsvStatementRequest = generateCsvStatementRequestDto.ToModel();

        var csv = await statementService.GenerateCsvAsync(generatedCsvStatementRequest, cancellationToken);

        return File(csv, MediaTypeNames.Text.Csv, $"{nameof(Statement)}_{generateCsvStatementRequestDto.AccountId}.csv");
    }
}
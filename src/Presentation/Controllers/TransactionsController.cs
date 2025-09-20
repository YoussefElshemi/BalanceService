using System.Net.Mime;
using Core.Interfaces;
using Core.ValueObjects;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Presentation.CustomBinding;
using Presentation.Mappers;
using Presentation.Mappers.Transactions;
using Presentation.Models;
using Presentation.Models.Transactions;

namespace Presentation.Controllers;

/// <summary>
/// Manages operations on transactions
/// </summary>
[ApiController]
[Route("transactions")]
[Produces(MediaTypeNames.Application.Json)]
[ProducesResponseType(StatusCodes.Status400BadRequest)]
[ProducesResponseType(StatusCodes.Status500InternalServerError)]
public class TransactionsController : Controller
{
    /// <summary>
    /// Creates a draft transaction.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(TransactionDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> CreateTransaction(
        [FromHybrid] CreateTransactionRequestDto createTransactionRequestDto,
        [FromServices] IValidator<CreateTransactionRequestDto> createTransactionRequestDtoValidator,
        [FromServices] ITransactionService transactionService,
        CancellationToken cancellationToken)
    {
        await createTransactionRequestDtoValidator.ValidateAndThrowAsync(createTransactionRequestDto, cancellationToken);

        var createTransactionRequest = createTransactionRequestDto.ToModel();

        var transaction = await transactionService.CreateAsync(createTransactionRequest, cancellationToken);

        return CreatedAtAction(
            actionName: nameof(GetTransactionById),
            controllerName: "Transactions", 
            routeValues: new { transaction.TransactionId },
            value: transaction.ToDto()
        );
    }
    
    /// <summary>
    /// Queries transactions with optional filters and pagination.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(PagedResultsDto<TransactionDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> QueryTransactions(
        [FromHybrid] QueryTransactionsRequestDto queryTransactionsRequestDto,
        [FromServices] IValidator<QueryTransactionsRequestDto> queryTransactionsRequestDtoValidator,
        [FromServices] ITransactionService transactionService,
        CancellationToken cancellationToken)
    {
        await queryTransactionsRequestDtoValidator.ValidateAndThrowAsync(queryTransactionsRequestDto, cancellationToken);
    
        var queryTransactionsRequest = queryTransactionsRequestDto.ToModel();
    
        var results = await transactionService.QueryAsync(queryTransactionsRequest, cancellationToken);
    
        return Ok(results.ToDto(x => x.ToDto()));
    }

    /// <summary>
    /// Gets a transaction by its ID.
    /// </summary>
    [HttpGet("{transactionId:guid}")]
    [ProducesResponseType(typeof(TransactionDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetTransactionById(
        [FromHybrid] GetTransactionRequestDto getTransactionRequestDto,
        [FromServices] ITransactionService transactionService,
        CancellationToken cancellationToken)
    {
        var transaction = await transactionService.GetByIdAsync(
            new TransactionId(getTransactionRequestDto.TransactionId),
            cancellationToken);

        return Ok(transaction.ToDto());
    }

    /// <summary>
    /// Posts a draft transaction by its ID.
    /// </summary>
    [HttpPost("{transactionId:guid}/post")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> PostTransactionById(
        [FromHybrid] PostTransactionRequestDto postTransactionRequestDto,
        [FromServices] ITransactionService transactionService,
        CancellationToken cancellationToken)
    {
        await transactionService.PostAsync(
            new TransactionId(postTransactionRequestDto.TransactionId),
            new Username(postTransactionRequestDto.Username),
            cancellationToken);

        return NoContent();
    }

    /// <summary>
    /// Reverses a posted transaction by its ID.
    /// </summary>
    [HttpPost("{transactionId:guid}/reverse")]
    [ProducesResponseType(typeof(TransactionDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> ReverseTransactionById(
        [FromHybrid] ReverseTransactionRequestDto reverseTransactionRequestDto,
        [FromServices] ITransactionService transactionService,
        CancellationToken cancellationToken)
    {
        var reversedTransaction = await transactionService.ReverseAsync(
            new TransactionId(reverseTransactionRequestDto.TransactionId),
            new Username(reverseTransactionRequestDto.Username),
            cancellationToken);

        return CreatedAtAction(
            actionName: nameof(GetTransactionById),
            controllerName: "Transactions", 
            routeValues: new { reversedTransaction.TransactionId },
            value: reversedTransaction.ToDto()
        );
    }

    /// <summary>|
    /// Gets a transaction's history by its ID.
    /// </summary>
    [HttpGet("{transactionId:guid}/history")]
    [ProducesResponseType(typeof(PagedResultsDto<ChangeEventDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetTransactionHistoryById(
        [FromHybrid] GetTransactionHistoryRequestDto getTransactionHistoryRequestDto,
        [FromServices] IValidator<GetHistoryRequestDto> getHistoryRequestDtoValidator,
        [FromServices] ITransactionService transactionService,
        CancellationToken cancellationToken)
    {
        await getHistoryRequestDtoValidator.ValidateAndThrowAsync(getTransactionHistoryRequestDto, cancellationToken);

        var getHistoryRequest = getTransactionHistoryRequestDto.ToModel(getTransactionHistoryRequestDto.TransactionId);

        var results = await transactionService.GetHistoryAsync(getHistoryRequest, cancellationToken);

        return Ok(results.ToDto(x => x.ToDto()));
        
    }

    /// <summary>
    /// Updates an existing draft transaction.
    /// </summary>
    [HttpPut("{transactionId:guid}")]
    [ProducesResponseType(typeof(TransactionDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> UpdateTransaction(
        [FromHybrid] UpdateTransactionRequestDto updateTransactionRequestDto,
        [FromServices] IValidator<UpdateTransactionRequestDto> updateTransactionRequestDtoValidator,
        [FromServices] ITransactionService transactionService,
        CancellationToken cancellationToken)
    {
        await updateTransactionRequestDtoValidator.ValidateAndThrowAsync(updateTransactionRequestDto, cancellationToken);

        var updateTransactionRequest = updateTransactionRequestDto.ToModel();
    
        var updatedTransaction = await transactionService.UpdateAsync(updateTransactionRequest, cancellationToken);
    
        return Ok(updatedTransaction.ToDto());
    }

    /// <summary>
    /// Deletes a draft transaction.
    /// </summary>
    [HttpDelete("{transactionId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> DeleteTransaction(
        [FromHybrid] DeleteTransactionRequestDto deleteTransactionRequestDto,
        [FromServices] ITransactionService transactionService,
        CancellationToken cancellationToken)
    {
        await transactionService.DeleteAsync(
            new TransactionId(deleteTransactionRequestDto.TransactionId),
            new Username(deleteTransactionRequestDto.Username),
            cancellationToken);

        return NoContent();
    }
}

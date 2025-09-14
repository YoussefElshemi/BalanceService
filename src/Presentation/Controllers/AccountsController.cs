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
/// Manages operations on accounts
/// </summary>
[ApiController]
[Route("accounts")]
[Produces("application/json")]
[ProducesResponseType(StatusCodes.Status400BadRequest)]
[ProducesResponseType(StatusCodes.Status500InternalServerError)]
public class AccountsController : Controller
{
    /// <summary>
    /// Creates a new account.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(AccountDto), StatusCodes.Status201Created)]
    public async Task<IActionResult> CreateAccount(
        [FromBody] CreateAccountRequestDto createAccountRequestDto,
        [FromHeader(Name = HeaderNames.CorrelationId)] Guid correlationId,
        [Required][FromHeader(Name = HeaderNames.Username)] string username,
        [FromServices] IValidator<CreateAccountRequestDto> createAccountRequestDtoValidator,
        [FromServices] IAccountService accountService,
        CancellationToken cancellationToken)
    {
        await createAccountRequestDtoValidator.ValidateAndThrowAsync(createAccountRequestDto, cancellationToken);

        var createAccountRequest = createAccountRequestDto.ToModel(username);

        var account = await accountService.CreateAsync(createAccountRequest, cancellationToken);

        return CreatedAtAction(
            actionName: nameof(GetAccountById),
            controllerName: "Accounts", 
            routeValues: new { account.AccountId },
            value: account.ToDto()
        );
    }

    /// <summary>
    /// Queries accounts with optional filters and pagination.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(PagedResultsDto<AccountDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> QueryAccounts(
        [FromQuery] QueryAccountsRequestDto queryAccountsRequestDto,
        [FromHeader(Name = HeaderNames.CorrelationId)] Guid correlationId,
        [FromServices] IValidator<QueryAccountsRequestDto> queryAccountsRequestDtoValidator,
        [FromServices] IAccountService accountService,
        CancellationToken cancellationToken)
    {
        await queryAccountsRequestDtoValidator.ValidateAndThrowAsync(queryAccountsRequestDto, cancellationToken);

        var queryAccountsRequest = queryAccountsRequestDto.ToModel();

        var results = await accountService.QueryAsync(queryAccountsRequest, cancellationToken);

        return Ok(results.ToDto(x => x.ToDto()));
    }

    /// <summary>
    /// Gets an account by its ID.
    /// </summary>
    [HttpGet("{accountId:guid}")]
    [ProducesResponseType(typeof(AccountDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetAccountById(
        [FromRoute] Guid accountId,
        [FromHeader(Name = HeaderNames.CorrelationId)] Guid correlationId,
        [FromServices] IAccountService accountService,
        CancellationToken cancellationToken)
    {
        var account = await accountService.GetByIdAsync(new AccountId(accountId), cancellationToken);

        return Ok(account.ToDto());
    }
    
    /// <summary>
    /// Activates an account by its ID.
    /// </summary>
    [HttpPost("{accountId:guid}/activate")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ActivateAccountById(
        [FromRoute] Guid accountId,
        [FromHeader(Name = HeaderNames.CorrelationId)] Guid correlationId,
        [Required][FromHeader(Name = HeaderNames.Username)] string username,
        [FromServices] IAccountService accountService,
        CancellationToken cancellationToken)
    {
        await accountService.ActivateAsync(new AccountId(accountId), new Username(username), cancellationToken);

        return NoContent();
    }
    
    /// <summary>
    /// Freezes an account by its ID.
    /// </summary>
    [HttpPost("{accountId:guid}/freeze")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> FreezeAccountById(
        [FromRoute] Guid accountId,
        [FromHeader(Name = HeaderNames.CorrelationId)] Guid correlationId,
        [Required][FromHeader(Name = HeaderNames.Username)] string username,
        [FromServices] IAccountService accountService,
        CancellationToken cancellationToken)
    {
        await accountService.FreezeAsync(new AccountId(accountId), new Username(username), cancellationToken);

        return NoContent();
    }

    /// <summary>
    /// Closes / marks an account for closure by its ID.
    /// </summary>
    [HttpPost("{accountId:guid}/close")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> CloseAccountById(
        [FromRoute] Guid accountId,
        [FromHeader(Name = HeaderNames.CorrelationId)] Guid correlationId,
        [Required][FromHeader(Name = HeaderNames.Username)] string username,
        [FromServices] IAccountService accountService,
        CancellationToken cancellationToken)
    {
        await accountService.CloseAsync(new AccountId(accountId), new Username(username), cancellationToken);

        return NoContent();
    }

    /// <summary>
    /// Gets an account's balances by its ID.
    /// </summary>
    [HttpGet("{accountId:guid}/balances")]
    [ProducesResponseType(typeof(AccountBalanceDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetAccountBalancesById(
        [FromRoute] Guid accountId,
        [FromHeader(Name = HeaderNames.CorrelationId)] Guid correlationId,
        [FromServices] IAccountService accountService,
        CancellationToken cancellationToken)
    {
        var balances = await accountService.GetBalancesByIdAsync(new AccountId(accountId), cancellationToken);

        return Ok(balances.ToDto());
    }

    
    // TODO: handle mapping entity -> domain column names, and enum values too
    
    /// <summary>|
    /// Gets an account's history by its ID.
    /// </summary>
    [HttpGet("{accountId:guid}/history")]
    [ProducesResponseType(typeof(AccountBalanceDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetAccountHistoryById(
        [FromRoute] Guid accountId,
        [FromHeader(Name = HeaderNames.CorrelationId)] Guid correlationId,
        [FromQuery] GetChangesRequestDto getChangesRequestDto,
        [FromServices] IValidator<GetChangesRequestDto> getChangesRequestDtoValidator,
        [FromServices] IAccountService accountService,
        CancellationToken cancellationToken)
    {
        await getChangesRequestDtoValidator.ValidateAndThrowAsync(getChangesRequestDto, cancellationToken);

        var getChangesRequest = getChangesRequestDto.ToModel(accountId);

        var results = await accountService.GetHistoryAsync(getChangesRequest, cancellationToken);

        return Ok(results.ToDto(x => x.ToDto()));
        
    }

    /// <summary>
    /// Updates an existing account.
    /// </summary>
    [HttpPut("{accountId:guid}")]
    [ProducesResponseType(typeof(AccountDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> UpdateAccount(
        [FromRoute] Guid accountId,
        [FromBody] UpdateAccountRequestDto updateAccountRequestDto,
        [FromHeader(Name = HeaderNames.CorrelationId)] Guid correlationId,
        [Required][FromHeader(Name = HeaderNames.Username)] string username,
        [FromServices] IValidator<UpdateAccountRequestDto> updateAccountRequestDtoValidator,
        [FromServices] IAccountService accountService,
        CancellationToken cancellationToken)
    {
        await updateAccountRequestDtoValidator.ValidateAndThrowAsync(updateAccountRequestDto, cancellationToken);

        var updateAccountRequest = updateAccountRequestDto.ToModel(username);

        var account = await accountService.UpdateAsync(new AccountId(accountId), updateAccountRequest, cancellationToken);

        return Ok(account.ToDto());
    }

    /// <summary>
    /// Deletes an account.
    /// </summary>
    [HttpDelete("{accountId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteAccount(
        [FromRoute] Guid accountId,
        [FromHeader(Name = HeaderNames.CorrelationId)] Guid correlationId,
        [Required][FromHeader(Name = HeaderNames.Username)] string username,
        [FromServices] IAccountService accountService,
        CancellationToken cancellationToken)
    {
        await accountService.DeleteAsync(new AccountId(accountId), new Username(username), cancellationToken);

        return NoContent();
    }
}

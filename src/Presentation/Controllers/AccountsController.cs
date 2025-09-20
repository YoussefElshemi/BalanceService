using System.Net.Mime;
using Core.Interfaces;
using Core.ValueObjects;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Presentation.CustomBinding;
using Presentation.Mappers;
using Presentation.Mappers.Accounts;
using Presentation.Models;
using Presentation.Models.Accounts;

namespace Presentation.Controllers;

/// <summary>
/// Manages operations on accounts
/// </summary>
[ApiController]
[Route("accounts")]
[Produces(MediaTypeNames.Application.Json)]
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
        [FromHybrid] CreateAccountRequestDto createAccountRequestDto,
        [FromServices] IValidator<CreateAccountRequestDto> createAccountRequestDtoValidator,
        [FromServices] IAccountService accountService,
        CancellationToken cancellationToken)
    {
        await createAccountRequestDtoValidator.ValidateAndThrowAsync(createAccountRequestDto, cancellationToken);

        var createAccountRequest = createAccountRequestDto.ToModel();

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
        [FromHybrid] QueryAccountsRequestDto queryAccountsRequestDto,
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
        [FromHybrid] GetAccountRequestDto getAccountRequestDto,
        [FromServices] IAccountService accountService,
        CancellationToken cancellationToken)
    {
        var account = await accountService.GetByIdAsync(new AccountId(getAccountRequestDto.AccountId), cancellationToken);

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
        [FromHybrid] ActivateAccountRequestDto activateAccountRequestDto,
        [FromServices] IAccountService accountService,
        CancellationToken cancellationToken)
    {
        await accountService.ActivateAsync(
            new AccountId(activateAccountRequestDto.AccountId),
            new Username(activateAccountRequestDto.Username),
            cancellationToken);

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
        [FromHybrid] FreezeAccountRequestDto freezeAccountRequestDto,
        [FromServices] IAccountService accountService,
        CancellationToken cancellationToken)
    {
        await accountService.FreezeAsync(
            new AccountId(freezeAccountRequestDto.AccountId),
            new Username(freezeAccountRequestDto.Username),
            cancellationToken);

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
        [FromHybrid] CloseAccountRequestDto closeAccountRequestDto,
        [FromServices] IAccountService accountService,
        CancellationToken cancellationToken)
    {
        await accountService.CloseAsync(
            new AccountId(closeAccountRequestDto.AccountId),
            new Username(closeAccountRequestDto.Username),
            cancellationToken);

        return NoContent();
    }

    /// <summary>
    /// Gets an account's balances by its ID.
    /// </summary>
    [HttpGet("{accountId:guid}/balances")]
    [ProducesResponseType(typeof(AccountBalanceDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetAccountBalancesById(
        [FromHybrid] GetAccountBalancesRequestDto getAccountBalancesRequestDto,
        [FromServices] IAccountService accountService,
        CancellationToken cancellationToken)
    {
        var balances = await accountService.GetBalancesByIdAsync(
            new AccountId(getAccountBalancesRequestDto.AccountId),
            cancellationToken);

        return Ok(balances.ToDto());
    }

    /// <summary>|
    /// Gets an account's history by its ID.
    /// </summary>
    [HttpGet("{accountId:guid}/history")]
    [ProducesResponseType(typeof(PagedResultsDto<ChangeEventDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetAccountHistoryById(
        [FromHybrid] GetAccountHistoryRequestDto getAccountHistoryRequestDto,
        [FromServices] IValidator<GetHistoryRequestDto> getHistoryRequestDtoValidator,
        [FromServices] IAccountService accountService,
        CancellationToken cancellationToken)
    {
        await getHistoryRequestDtoValidator.ValidateAndThrowAsync(getAccountHistoryRequestDto, cancellationToken);

        var getHistoryRequest = getAccountHistoryRequestDto.ToModel(getAccountHistoryRequestDto.AccountId);

        var results = await accountService.GetHistoryAsync(getHistoryRequest, cancellationToken);

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
        [FromHybrid] UpdateAccountRequestDto updateAccountRequestDto,
        [FromServices] IValidator<UpdateAccountRequestDto> updateAccountRequestDtoValidator,
        [FromServices] IAccountService accountService,
        CancellationToken cancellationToken)
    {
        await updateAccountRequestDtoValidator.ValidateAndThrowAsync(updateAccountRequestDto, cancellationToken);

        var updateAccountRequest = updateAccountRequestDto.ToModel();

        var account = await accountService.UpdateAsync(updateAccountRequest, cancellationToken);

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
        [FromHybrid] DeleteAccountRequestDto deleteAccountRequestDto,
        [FromServices] IAccountService accountService,
        CancellationToken cancellationToken)
    {
        await accountService.DeleteAsync(
            new AccountId(deleteAccountRequestDto.AccountId),
            new Username(deleteAccountRequestDto.Username),
            cancellationToken);

        return NoContent();
    }
}

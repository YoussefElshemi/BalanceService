using Core.Interfaces;
using Core.ValueObjects;
using FluentValidation;
using Presentation.Models;

namespace Presentation.Validators;

public class CreateHoldRequestDtoValidator : AbstractValidator<CreateHoldRequestDto>
{
    private readonly IAccountService _accountService;

    public CreateHoldRequestDtoValidator(
        IAccountService accountService,
        TimeProvider timeProvider)
    {
        _accountService = accountService;

        RuleFor(x => x.AccountId)
            .NotEmpty()
            .MustAsync(AccountExists);

        RuleFor(x => x.Amount)
            .NotEmpty()
            .GreaterThan(0);

        RuleFor(x => x.CurrencyCode)
            .NotEmpty()
            .IsInEnum();

        RuleFor(x => x.IdempotencyKey)
            .NotEmpty();

        RuleFor(x => x.Type)
            .NotEmpty()
            .IsInEnum();

        RuleFor(x => x.ExpiresAt)
            .GreaterThanOrEqualTo(timeProvider.GetUtcNow())
            .When(x => x.ExpiresAt.HasValue);

        RuleFor(x => x.Description)
            .MaximumLength(256)
            .When(x => x.Description != null);
        
        RuleFor(x => x.Reference)
            .MaximumLength(256)
            .When(x => x.Reference != null);
    }

    private Task<bool> AccountExists(Guid accountId, CancellationToken cancellationToken)
    {
        return _accountService.ExistsAsync(new AccountId(accountId), cancellationToken);
    }
}

using Core.Interfaces;
using Core.ValueObjects;
using FluentValidation;
using Presentation.Models.Holds;

namespace Presentation.Validators.Holds;

public class CreateHoldRequestDtoValidator : AbstractValidator<CreateHoldRequestDto>
{
    public CreateHoldRequestDtoValidator(
        IAccountService accountService,
        ICurrencyService currencyService,
        TimeProvider timeProvider)
    {
        RuleFor(x => x.AccountId)
            .NotEmpty()
            .MustAsync((accountId, cancellationToken) => accountService.ExistsAsync(new AccountId(accountId), cancellationToken))
            .WithMessage(x => $"Account ({x.AccountId}) does not exist");

        RuleFor(x => x.Amount)
            .NotEmpty()
            .GreaterThan(0)
            .Must((x, y) => currencyService.IsValid(x.CurrencyCode, y))
            .WithMessage(x => $"Max {currencyService.GetMaxNumberOfDecimalPlaces(x.CurrencyCode)} decimal places allowed");

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
}

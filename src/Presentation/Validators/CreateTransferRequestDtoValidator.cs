using Core.Interfaces;
using Core.ValueObjects;
using FluentValidation;
using Presentation.Models;

namespace Presentation.Validators;

public class CreateTransferRequestDtoValidator : AbstractValidator<CreateTransferRequestDto>
{
    public CreateTransferRequestDtoValidator(IAccountService accountService)
    {
        RuleFor(x => x.DebitAccountId)
            .NotEmpty()
            .MustAsync((accountId, cancellationToken) => accountService.ExistsAsync(new AccountId(accountId), cancellationToken))
            .WithMessage(x => $"Account ({x.DebitAccountId}) does not exist");

        RuleFor(x => x.CreditAccountId)
            .NotEmpty()
            .MustAsync((accountId, cancellationToken) => accountService.ExistsAsync(new AccountId(accountId), cancellationToken))
            .WithMessage(x => $"Account ({x.CreditAccountId}) does not exist")
            .NotEqual(x => x.DebitAccountId)
            .WithMessage(x => $"{nameof(x.CreditAccountId)} and {nameof(x.DebitAccountId)} cannot be the same");

        RuleFor(x => x.Amount)
            .NotEmpty()
            .GreaterThan(0);

        RuleFor(x => x.CurrencyCode)
            .NotEmpty()
            .IsInEnum();

        RuleFor(x => x.DebitIdempotencyKey)
            .NotEmpty();

        RuleFor(x => x.CreditIdempotencyKey)
            .NotEmpty();

        RuleFor(x => x.Description)
            .MaximumLength(256)
            .When(x => x.Description != null);
        
        RuleFor(x => x.Reference)
            .MaximumLength(256)
            .When(x => x.Reference != null);
    }
}

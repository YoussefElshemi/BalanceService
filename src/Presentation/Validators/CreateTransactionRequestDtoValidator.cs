using Core.Interfaces;
using Core.ValueObjects;
using FluentValidation;
using Presentation.Models;

namespace Presentation.Validators;

public class CreateTransactionRequestDtoValidator : AbstractValidator<CreateTransactionRequestDto>
{
    public CreateTransactionRequestDtoValidator(IAccountService accountService)
    {
        RuleFor(x => x.AccountId)
            .NotEmpty()
            .MustAsync((accountId, cancellationToken) => accountService.ExistsAsync(new AccountId(accountId), cancellationToken))
            .WithMessage(x => $"Account ({x.AccountId}) does not exist");

        RuleFor(x => x.Amount)
            .NotEmpty()
            .GreaterThan(0);

        RuleFor(x => x.CurrencyCode)
            .NotEmpty()
            .IsInEnum();
        
        RuleFor(x => x.Direction)
            .NotEmpty()
            .IsInEnum();

        RuleFor(x => x.IdempotencyKey)
            .NotEmpty();

        RuleFor(x => x.Type)
            .NotEmpty()
            .IsInEnum();

        RuleFor(x => x.Source)
            .NotEmpty()
            .IsInEnum();
        
        RuleFor(x => x.Description)
            .MaximumLength(256)
            .When(x => x.Description != null);
        
        RuleFor(x => x.Reference)
            .MaximumLength(256)
            .When(x => x.Reference != null);
    }
}

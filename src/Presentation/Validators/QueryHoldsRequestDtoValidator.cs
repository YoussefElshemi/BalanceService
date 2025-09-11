using Core.Interfaces;
using Core.ValueObjects;
using FluentValidation;
using Presentation.Models;

namespace Presentation.Validators;

public class QueryHoldsRequestDtoValidator : AbstractValidator<QueryHoldsRequestDto>
{
    public QueryHoldsRequestDtoValidator(
        IAccountService accountService,
        ITransactionService transactionService)
    {
        RuleFor(x => x.PageSize)
            .NotEmpty()
            .InclusiveBetween(1, 100);

        RuleFor(x => x.PageNumber)
            .NotEmpty()
            .GreaterThan(0);

        RuleFor(x => x.AccountId)
            .NotEmpty()
            .MustAsync((accountId, cancellationToken) => accountService.ExistsAsync(new AccountId(accountId!.Value), cancellationToken))
            .WithMessage(x => $"Account ({x.AccountId}) does not exist")
            .When(x => x.AccountId.HasValue);

        RuleFor(x => x.CurrencyCode)
            .IsInEnum()
            .When(x => x.CurrencyCode != null);

        RuleFor(x => x.Amount)
            .GreaterThan(0)
            .When(x => x.Amount != null);

        RuleFor(x => x.SettledTransactionId)
            .MustAsync((transactionId, cancellationToken) => transactionService.ExistsAsync(new TransactionId(transactionId!.Value), cancellationToken))
            .WithMessage(x => $"Transaction ({x.SettledTransactionId}) does not exist")
            .When(x => x.SettledTransactionId != null);

        RuleFor(x => x.Type)
            .IsInEnum()
            .When(x => x.Type != null);
        
        RuleFor(x => x.Status)
            .IsInEnum()
            .When(x => x.Status != null);
        
        RuleFor(x => x.Source)
            .IsInEnum()
            .When(x => x.Source != null);

        RuleFor(x => x.Description)
            .MaximumLength(256)
            .When(x => !string.IsNullOrWhiteSpace(x.Description));

        RuleFor(x => x.Reference)
            .MaximumLength(256)
            .When(x => !string.IsNullOrWhiteSpace(x.Reference));
    }
}

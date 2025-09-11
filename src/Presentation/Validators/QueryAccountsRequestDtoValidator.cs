using Core.Constants;
using Core.Extensions;
using Core.Interfaces;
using Core.ValueObjects;
using FluentValidation;
using Presentation.Models;

namespace Presentation.Validators;

public class QueryAccountsRequestDtoValidator : AbstractValidator<QueryAccountsRequestDto>
{
    public QueryAccountsRequestDtoValidator(IAccountService accountService)
    {
        RuleFor(x => x.PageSize)
            .NotEmpty()
            .InclusiveBetween(1, 100);

        RuleFor(x => x.PageNumber)
            .NotEmpty()
            .GreaterThan(0);

        RuleFor(x => x.AccountName)
            .Matches(RegexConstants.AlphaNumericRegex)
            .When(x => x.AccountName != null);

        RuleFor(x => x.CurrencyCode)
            .IsInEnum()
            .When(x => x.CurrencyCode != null);

        RuleFor(x => x.AccountType)
            .IsInEnum()
            .When(x => x.AccountType != null);
        
        RuleFor(x => x.Metadata)
            .Must(x => x.BeValidJsonObject())
            .WithMessage("Metadata must be a valid JSON object.")
            .When(x => x.Metadata != null);

        RuleFor(x => x.ParentAccountId)
            .NotEmpty()
            .MustAsync((accountId, cancellationToken) => accountService.ExistsAsync(new AccountId(accountId!.Value), cancellationToken))
            .WithMessage(x => $"Parent Account ({x.ParentAccountId}) does not exist")
            .When(x => x.ParentAccountId.HasValue);

        RuleFor(x => x.ParentAccountName)
            .Matches(RegexConstants.AlphaNumericRegex)
            .When(x => x.ParentAccountName != null);

        RuleFor(x => x)
            .Must(x => !(x.ParentAccountId.HasValue && x.ParentAccountName != null))
            .WithMessage($"You cannot provide both {nameof(QueryAccountsRequestDto.ParentAccountId)} and " +
                         $"{nameof(QueryAccountsRequestDto.ParentAccountId)}.");
    }
}

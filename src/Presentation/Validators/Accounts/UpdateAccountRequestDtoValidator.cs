using Core.Constants;
using Core.Extensions;
using Core.Interfaces;
using Core.ValueObjects;
using FluentValidation;
using Presentation.Models.Accounts;

namespace Presentation.Validators.Accounts;

public class UpdateAccountRequestDtoValidator : AbstractValidator<UpdateAccountRequestDto>
{
    public UpdateAccountRequestDtoValidator(IAccountService accountService)
    {
        RuleFor(x => x.AccountName)
            .NotEmpty()
            .Length(2, 100)
            .Matches(RegexConstants.AlphaNumericRegex);

        RuleFor(x => x.CurrencyCode)
            .NotEmpty()
            .IsInEnum();

        RuleFor(x => x.AccountType)
            .NotEmpty()
            .IsInEnum();
        
        RuleFor(x => x.Metadata)
            .Must(x => x.BeValidJsonObject())
            .WithMessage("Metadata must be a valid JSON object.")
            .When(x => x.Metadata != null);

        RuleFor(x => x.ParentAccountId)
            .NotEmpty()
            .MustAsync((accountId, cancellationToken) => accountService.ExistsAsync(new AccountId(accountId!.Value), cancellationToken))
            .WithMessage(x => $"Parent Account ({x.ParentAccountId}) does not exist")
            .When(x => x.ParentAccountId.HasValue);
    }
}

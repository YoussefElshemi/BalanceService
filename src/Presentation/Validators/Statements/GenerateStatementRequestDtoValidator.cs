using Core.Interfaces;
using Core.ValueObjects;
using FluentValidation;
using Presentation.Models.Statements;

namespace Presentation.Validators.Statements;

public class GenerateStatementRequestDtoValidator : AbstractValidator<GenerateStatementRequestDto>
{
    private const int MaximumNumberOfDays = 90;

    public GenerateStatementRequestDtoValidator(IAccountService accountService)
    {
        RuleFor(x => x.AccountId)
            .NotEmpty()
            .MustAsync((accountId, cancellationToken) => accountService.ExistsAsync(new AccountId(accountId), cancellationToken))
            .WithMessage(x => $"Account ({x.AccountId}) does not exist");

        RuleFor(x => x.FromDate)
            .NotEmpty();

        RuleFor(x => x.ToDate)
            .NotEmpty()
            .GreaterThanOrEqualTo(x => x.FromDate)
            .Must((x, _) => x.ToDate.DayNumber - x.FromDate.DayNumber <= MaximumNumberOfDays)
            .WithMessage($"Date {nameof(Range)} must be less than or equal to {MaximumNumberOfDays} days");

        RuleFor(x => x.Direction)
            .IsInEnum()
            .When(x => x.Direction != null);
    }
}

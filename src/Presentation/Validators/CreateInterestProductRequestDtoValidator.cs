using FluentValidation;
using Presentation.Models;

namespace Presentation.Validators;

public class CreateInterestProductRequestDtoValidator : AbstractValidator<CreateInterestProductRequestDto>
{
    public CreateInterestProductRequestDtoValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .Length(2, 100);

        RuleFor(x => x.AnnualInterestRate)
            .NotEmpty();

        RuleFor(x => x.InterestPayoutFrequency)
            .NotEmpty()
            .IsInEnum();

        RuleFor(x => x.AccrualBasis)
            .NotEmpty();
    }
}

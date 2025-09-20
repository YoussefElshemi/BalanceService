using FluentValidation;
using Presentation.Models.InterestProducts;

namespace Presentation.Validators.InterestProducts;

public class UpdateInterestProductRequestDtoValidator : AbstractValidator<UpdateInterestProductRequestDto>
{
    public UpdateInterestProductRequestDtoValidator()
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

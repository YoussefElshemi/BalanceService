using FluentValidation;
using Presentation.Models;

namespace Presentation.Validators;

public class CreateInterestProductAccountLinkRequestDtoValidator : AbstractValidator<CreateInterestProductAccountLinkRequestDto>
{
    public CreateInterestProductAccountLinkRequestDtoValidator()
    {
        RuleFor(x => x.InterestProductId)
            .NotEmpty();
    }
}

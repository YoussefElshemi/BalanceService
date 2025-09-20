using FluentValidation;
using Presentation.Models.InterestProductAccountLinks;

namespace Presentation.Validators.InterestProductAccountLinks;

public class CreateInterestProductAccountLinkRequestDtoValidator : AbstractValidator<CreateInterestProductAccountLinkRequestDto>
{
    public CreateInterestProductAccountLinkRequestDtoValidator(TimeProvider timeProvider)
    {
        RuleFor(x => x.InterestProductId)
            .NotEmpty();
        
        RuleFor(x => x.ExpiresAt)
            .GreaterThanOrEqualTo(timeProvider.GetUtcNow())
            .When(x => x.ExpiresAt.HasValue);
    }
}

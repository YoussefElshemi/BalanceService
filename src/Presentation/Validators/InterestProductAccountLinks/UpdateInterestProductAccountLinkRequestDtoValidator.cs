using FluentValidation;
using Presentation.Models.InterestProductAccountLinks;

namespace Presentation.Validators.InterestProductAccountLinks;

public class UpdateInterestProductAccountLinkRequestDtoValidator : AbstractValidator<UpdateInterestProductAccountLinkRequestDto>
{
    public UpdateInterestProductAccountLinkRequestDtoValidator(TimeProvider timeProvider)
    {
        RuleFor(x => x.ExpiresAt)
            .GreaterThanOrEqualTo(timeProvider.GetUtcNow())
            .When(x => x.ExpiresAt.HasValue);
    }
}

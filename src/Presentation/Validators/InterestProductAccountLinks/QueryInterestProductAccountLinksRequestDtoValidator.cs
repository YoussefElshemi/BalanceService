using FluentValidation;
using Presentation.Models.InterestProductAccountLinks;

namespace Presentation.Validators.InterestProductAccountLinks;

public class QueryInterestProductAccountLinksRequestDtoValidator : AbstractValidator<QueryInterestProductAccountLinksRequestDto>
{
    public QueryInterestProductAccountLinksRequestDtoValidator()
    {
        RuleFor(x => x.PageSize)
            .NotEmpty()
            .InclusiveBetween(1, 100);

        RuleFor(x => x.PageNumber)
            .NotEmpty()
            .GreaterThan(0);
    }
}

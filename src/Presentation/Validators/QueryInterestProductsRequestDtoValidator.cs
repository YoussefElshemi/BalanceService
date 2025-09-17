using Core.Constants;
using Core.Extensions;
using Core.Interfaces;
using Core.ValueObjects;
using FluentValidation;
using Presentation.Models;

namespace Presentation.Validators;

public class QueryInterestProductsRequestDtoValidator : AbstractValidator<QueryInterestProductsRequestDto>
{
    public QueryInterestProductsRequestDtoValidator()
    {
        RuleFor(x => x.PageSize)
            .NotEmpty()
            .InclusiveBetween(1, 100);

        RuleFor(x => x.PageNumber)
            .NotEmpty()
            .GreaterThan(0);

        RuleFor(x => x.InterestPayoutFrequency)
            .IsInEnum()
            .When(x => x.InterestPayoutFrequency != null);
    }
}

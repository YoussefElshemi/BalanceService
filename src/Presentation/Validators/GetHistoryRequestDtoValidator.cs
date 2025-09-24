using FluentValidation;
using Presentation.Models;

namespace Presentation.Validators;

public class GetHistoryRequestDtoValidator : AbstractValidator<GetHistoryRequestDto>
{
    public GetHistoryRequestDtoValidator()
    {
        RuleFor(x => x.PageSize)
            .NotEmpty()
            .InclusiveBetween(1, 100);

        RuleFor(x => x.PageNumber)
            .NotEmpty()
            .GreaterThan(0);
    }
}

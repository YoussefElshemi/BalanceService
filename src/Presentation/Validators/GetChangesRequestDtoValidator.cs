using Core.Interfaces;
using FluentValidation;
using Presentation.Models;

namespace Presentation.Validators;

public class GetChangesRequestDtoValidator : AbstractValidator<GetChangesRequestDto>
{
    public GetChangesRequestDtoValidator(IAccountService accountService)
    {
        RuleFor(x => x.PageSize)
            .NotEmpty()
            .InclusiveBetween(1, 100);

        RuleFor(x => x.PageNumber)
            .NotEmpty()
            .GreaterThan(0);
    }
}

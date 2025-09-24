using FluentValidation;
using Presentation.Models;

namespace Presentation.Validators;

public class BaseWriteRequestDtoValidator : AbstractValidator<BaseWriteRequestDto>
{
    public BaseWriteRequestDtoValidator()
    {
        Include(new BaseReadRequestDtoValidator());

        RuleFor(x => x.Username)
            .NotEmpty()
            .Length(3, 100);
    }
}

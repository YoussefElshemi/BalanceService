using FluentValidation;
using Presentation.Models;

namespace Presentation.Validators;

public class BaseReadRequestDtoValidator : AbstractValidator<BaseReadRequestDto>
{
    public BaseReadRequestDtoValidator()
    {
        RuleFor(x => x.CorrelationId)
            .NotEmpty()
            .When(x => x.CorrelationId != null);
    }
}

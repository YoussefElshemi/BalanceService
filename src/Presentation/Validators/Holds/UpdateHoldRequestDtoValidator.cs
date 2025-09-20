using FluentValidation;
using Presentation.Models.Holds;

namespace Presentation.Validators.Holds;

public class UpdateHoldRequestDtoValidator : AbstractValidator<UpdateHoldRequestDto>
{
    public UpdateHoldRequestDtoValidator(TimeProvider timeProvider)
    {
        RuleFor(x => x.Type)
            .NotEmpty()
            .IsInEnum();

        RuleFor(x => x.ExpiresAt)
            .GreaterThanOrEqualTo(timeProvider.GetUtcNow())
            .When(x => x.ExpiresAt.HasValue);

        RuleFor(x => x.Description)
            .MaximumLength(256)
            .When(x => !string.IsNullOrWhiteSpace(x.Description));
        
        RuleFor(x => x.Reference)
            .MaximumLength(256)
            .When(x => !string.IsNullOrWhiteSpace(x.Reference));
    }
}

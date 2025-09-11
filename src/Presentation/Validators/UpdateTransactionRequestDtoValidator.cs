using Core.Interfaces;
using Core.ValueObjects;
using FluentValidation;
using Presentation.Models;

namespace Presentation.Validators;

public class UpdateTransactionRequestDtoValidator : AbstractValidator<UpdateTransactionRequestDto>
{
    public UpdateTransactionRequestDtoValidator()
    {
        RuleFor(x => x.Type)
            .NotEmpty()
            .IsInEnum();

        RuleFor(x => x.Source)
            .NotEmpty()
            .IsInEnum();
        
        RuleFor(x => x.Description)
            .MaximumLength(256)
            .When(x => !string.IsNullOrWhiteSpace(x.Description));
        
        RuleFor(x => x.Reference)
            .MaximumLength(256)
            .When(x => !string.IsNullOrWhiteSpace(x.Reference));
    }
}

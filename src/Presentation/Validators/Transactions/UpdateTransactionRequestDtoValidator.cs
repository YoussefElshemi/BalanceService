using FluentValidation;
using Presentation.Models.Transactions;

namespace Presentation.Validators.Transactions;

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

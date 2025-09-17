using Core.Models;
using Core.ValueObjects;
using Presentation.Models;

namespace Presentation.Mappers;

public static class CreateInterestProductRequestDtoMapper
{
    public static CreateInterestProductRequest ToModel(this CreateInterestProductRequestDto createInterestProductRequestDto, string username)
    {
        return new CreateInterestProductRequest
        {
            Name = new InterestProductName(createInterestProductRequestDto.Name),
            AnnualInterestRate = new InterestRate(createInterestProductRequestDto.AnnualInterestRate),
            InterestPayoutFrequency = createInterestProductRequestDto.InterestPayoutFrequency,
            AccrualBasis = new AccrualBasis(createInterestProductRequestDto.AccrualBasis),
            CreatedBy = new Username(username)
        };
    }
}
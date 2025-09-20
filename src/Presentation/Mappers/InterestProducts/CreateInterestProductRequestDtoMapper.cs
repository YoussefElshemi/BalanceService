using Core.Models;
using Core.ValueObjects;
using Presentation.Models.InterestProducts;

namespace Presentation.Mappers.InterestProducts;

public static class CreateInterestProductRequestDtoMapper
{
    public static CreateInterestProductRequest ToModel(this CreateInterestProductRequestDto createInterestProductRequestDto)
    {
        return new CreateInterestProductRequest
        {
            Name = new InterestProductName(createInterestProductRequestDto.Name),
            AnnualInterestRate = new InterestRate(createInterestProductRequestDto.AnnualInterestRate),
            InterestPayoutFrequency = createInterestProductRequestDto.InterestPayoutFrequency,
            AccrualBasis = new AccrualBasis(createInterestProductRequestDto.AccrualBasis),
            CreatedBy = new Username(createInterestProductRequestDto.Username)
        };
    }
}
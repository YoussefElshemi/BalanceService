using Core.Models;
using Core.ValueObjects;
using Presentation.Models.InterestProducts;

namespace Presentation.Mappers.InterestProducts;

public static class UpdateInterestProductRequestDtoMapper
{
    public static UpdateInterestProductRequest ToModel(this UpdateInterestProductRequestDto updateInterestProductRequestDto)
    {
        return new UpdateInterestProductRequest
        {
            InterestProductId = new InterestProductId(updateInterestProductRequestDto.InterestProductId),
            Name = new InterestProductName(updateInterestProductRequestDto.Name),
            AnnualInterestRate = new InterestRate(updateInterestProductRequestDto.AnnualInterestRate),
            InterestPayoutFrequency = updateInterestProductRequestDto.InterestPayoutFrequency,
            AccrualBasis = new AccrualBasis(updateInterestProductRequestDto.AccrualBasis),
            UpdatedBy = new Username(updateInterestProductRequestDto.Username)
        };
    }
}
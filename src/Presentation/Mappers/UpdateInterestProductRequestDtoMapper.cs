using Core.Models;
using Core.ValueObjects;
using Presentation.Models;

namespace Presentation.Mappers;

public static class UpdateInterestProductRequestDtoMapper
{
    public static UpdateInterestProductRequest ToModel(this UpdateInterestProductRequestDto createInterestProductRequestDto, string username)
    {
        return new UpdateInterestProductRequest
        {
            Name = new InterestProductName(createInterestProductRequestDto.Name),
            AnnualInterestRate = new InterestRate(createInterestProductRequestDto.AnnualInterestRate),
            InterestPayoutFrequency = createInterestProductRequestDto.InterestPayoutFrequency,
            AccrualBasis = new AccrualBasis(createInterestProductRequestDto.AccrualBasis),
            UpdatedBy = new Username(username)
        };
    }
}
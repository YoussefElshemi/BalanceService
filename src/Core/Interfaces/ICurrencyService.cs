using Core.Enums;

namespace Core.Interfaces;

public interface ICurrencyService
{
    int GetMaxNumberOfDecimalPlaces(CurrencyCode currencyCode);
    bool IsValid(CurrencyCode currencyCode, decimal amount);
    decimal Round(CurrencyCode currencyCode, decimal amount);
    string Format(CurrencyCode currencyCode, decimal amount);
}
using System.Globalization;
using Core.Configs;
using Core.Enums;
using Core.Interfaces;
using Microsoft.Extensions.Options;

namespace Core.Services;

public class CurrencyService(IOptions<AppConfig> appConfig) : ICurrencyService
{
    private const int DefaultNumberOfDecimalPlaces = 2;
    private readonly Dictionary<CurrencyCode, int> _currencyDecimalPlaceConfiguration = 
        appConfig.Value.CurrencyDecimalPlaceConfiguration;

    public int GetMaxNumberOfDecimalPlaces(CurrencyCode currencyCode)
    {
        return _currencyDecimalPlaceConfiguration.GetValueOrDefault(currencyCode, DefaultNumberOfDecimalPlaces);
    }

    public bool IsValid(CurrencyCode currencyCode, decimal amount)
    {
        var maxNumberOfDecimalPlaces = GetMaxNumberOfDecimalPlaces(currencyCode);
        return amount.Scale <= maxNumberOfDecimalPlaces;
    }

    public decimal Round(CurrencyCode currencyCode, decimal amount)
    {
        var maxNumberOfDecimalPlaces = GetMaxNumberOfDecimalPlaces(currencyCode);
        var rounded = Math.Round(amount, maxNumberOfDecimalPlaces);

        return rounded;
    }

    public string Format(CurrencyCode currencyCode, decimal amount)
    {
        var maxNumberOfDecimalPlaces = GetMaxNumberOfDecimalPlaces(currencyCode);
        var formatted =  amount.ToString($"N{maxNumberOfDecimalPlaces}", CultureInfo.InvariantCulture);

        return formatted;
    }
}
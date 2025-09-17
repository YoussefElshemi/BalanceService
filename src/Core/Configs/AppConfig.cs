using Core.Enums;
using Environment = Core.Enums.Environment;

namespace Core.Configs;

public record AppConfig
{
    public string Version { get; init; } = null!;
    public Environment Environment { get; init; }
    public ObservabilityConfig ObservabilityConfig { get; init; } = null!;
    public Dictionary<CurrencyCode, int> CurrencyDecimalPlaceConfiguration { get; init; } = null!;
    public HoldExpiryConfig HoldExpiryConfig { get; init; } = null!;
    public InterestAccrualJobConfig InterestAccrualJobConfig { get; init; } = null!;
}
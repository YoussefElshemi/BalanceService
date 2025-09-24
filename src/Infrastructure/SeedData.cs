using System.Collections.Immutable;
using Core.Enums;
using Infrastructure.Entities;

namespace Infrastructure;

public static class SeedData
{
    public static readonly ImmutableArray<AccountTypeEntity> AccountTypes =
    [
        ..Enum.GetValues<AccountType>()
            .Select(x => new AccountTypeEntity
            {
                AccountTypeId = (int)x,
                Name = x.ToString()
            })
    ];

    public static readonly ImmutableArray<TransactionTypeEntity> TransactionTypes =
    [
        ..Enum.GetValues<TransactionType>()
            .Select(x => new TransactionTypeEntity
            {
                TransactionTypeId = (int)x,
                Name = x.ToString()
            })
    ];

    public static readonly ImmutableArray<TransactionStatusEntity> TransactionStatuses =
    [
        ..Enum.GetValues<TransactionStatus>()
            .Select(x => new TransactionStatusEntity
            {
                TransactionStatusId = (int)x,
                Name = x.ToString()
            })
    ];

    public static readonly ImmutableArray<TransactionDirectionEntity> TransactionDirections =
    [
        ..Enum.GetValues<TransactionDirection>()
            .Select(x => new TransactionDirectionEntity
            {
                TransactionDirectionId = (int)x,
                Name = x.ToString()
            })
    ];

    public static readonly ImmutableArray<TransactionSourceEntity> TransactionSources =
    [
        ..Enum.GetValues<TransactionSource>()
            .Select(x => new TransactionSourceEntity
            {
                TransactionSourceId = (int)x,
                Name = x.ToString()
            })
    ];

    public static readonly ImmutableArray<HoldSourceEntity> HoldSources =
    [
        ..Enum.GetValues<HoldSource>()
            .Select(x => new HoldSourceEntity
            {
                HoldSourceId = (int)x,
                Name = x.ToString()
            })
    ];

    public static readonly ImmutableArray<HoldStatusEntity> HoldStatuses =
    [
        ..Enum.GetValues<HoldStatus>()
            .Select(x => new HoldStatusEntity
            {
                HoldStatusId = (int)x,
                Name = x.ToString()
            })
    ];

    public static readonly ImmutableArray<HoldTypeEntity> HoldTypes =
    [
        ..Enum.GetValues<HoldType>()
            .Select(x => new HoldTypeEntity
            {
                HoldTypeId = (int)x,
                Name = x.ToString()
            })
    ];

    public static readonly ImmutableArray<AccountStatusEntity> AccountStatuses =
    [
        ..Enum.GetValues<AccountStatus>()
            .Select(x => new AccountStatusEntity
            {
                AccountStatusId = (int)x,
                Name = x.ToString()
            })
    ];
    
    public static readonly ImmutableArray<HistoryTypeEntity> HistoryTypes =
    [
        ..Enum.GetValues<HistoryType>()
            .Select(x => new HistoryTypeEntity
            {
                HistoryTypeId = (int)x,
                Name = x.ToString()
            })
    ];

    public static readonly ImmutableArray<ProcessingStatusEntity> ProcessingStatuses =
    [
        ..Enum.GetValues<ProcessingStatus>()
            .Select(x => new ProcessingStatusEntity
            {
                ProcessingStatusId = (int)x,
                Name = x.ToString()
            })
    ];
    
    public static readonly ImmutableArray<InterestPayoutFrequencyEntity> InterestPayoutFrequencies =
    [
        ..Enum.GetValues<InterestPayoutFrequency>()
            .Select(x => new InterestPayoutFrequencyEntity
            {
                InterestPayoutFrequencyId = (int)x,
                Name = x.ToString()
            })
    ];
}
using Core.Enums;
using Core.Interfaces;
using Core.Models;
using Core.ValueObjects;
using Infrastructure.Entities;
using Infrastructure.Entities.History;

namespace Infrastructure.Services;

public class AccountHistoryService(
    IHistoryRepository<AccountHistoryEntity, AccountHistory> repository)
    : HistoryService<AccountHistoryEntity, AccountHistory>(repository)
{
    protected override Dictionary<ChangeEventField, ChangeEventField> FieldMappings { get; } = new()
    {
        { new ChangeEventField(nameof(AccountEntity.AccountTypeId)), new ChangeEventField(nameof(Account.Type)) },
        { new ChangeEventField(nameof(AccountEntity.AccountStatusId)), new ChangeEventField(nameof(Account.Status)) }
    };

    protected override Dictionary<string, Func<ChangeEventValue?, ChangeEventValue?>> ValueMappers { get; } = new()
    {
        { nameof(AccountEntity.AccountTypeId), v => Enum.TryParse<AccountType>(v, out var value)
            ? new ChangeEventValue(value.ToString())
            : v
        },
        { nameof(AccountEntity.AccountStatusId), v => Enum.TryParse<AccountStatus>(v, out var value)
            ? new ChangeEventValue(value.ToString())
            : v
        }
    };
}
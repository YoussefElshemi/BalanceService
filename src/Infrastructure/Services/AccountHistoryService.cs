using Core.Enums;
using Core.Interfaces;
using Core.Models;
using Infrastructure.Entities;
using Infrastructure.Entities.History;

namespace Infrastructure.Services;

public class AccountHistoryService(
    IHistoryRepository<AccountHistoryEntity, AccountHistory> repository)
    : HistoryService<AccountHistoryEntity, AccountHistory>(repository)
{
    protected override Dictionary<string, string> FieldMappings { get; } = new()
    {
        { nameof(AccountEntity.AccountTypeId), nameof(Account.Type) },
        { nameof(AccountEntity.AccountStatusId), nameof(Account.Status) }
    };

    protected override Dictionary<string, Func<string?, string?>> ValueMappers { get; } = new()
    {
        { nameof(AccountEntity.AccountTypeId), v => Enum.TryParse<AccountType>(v, out var type) ? type.ToString() : v },
        { nameof(AccountEntity.AccountStatusId), v => Enum.TryParse<AccountStatus>(v, out var status) ? status.ToString() : v }
    };
}
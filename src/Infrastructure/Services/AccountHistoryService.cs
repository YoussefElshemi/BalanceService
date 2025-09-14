using Core.Enums;
using Core.Interfaces;
using Core.Models;
using Infrastructure.Entities;
using Infrastructure.Entities.History;

namespace Infrastructure.Services;

public class AccountHistoryService(
    IHistoryRepository<AccountHistoryEntity, AccountHistory> accountHistoryRepository) : IHistoryService<AccountHistory>
{
    private static readonly Dictionary<string, string> FieldMappings = new()
    {
        { nameof(AccountEntity.AccountTypeId), nameof(Account.AccountType) },
        { nameof(AccountEntity.AccountStatusId), nameof(Account.AccountStatus) }
    };

    public Task<int> CountChangesAsync(GetChangesRequest getChangesRequest, CancellationToken cancellationToken)
    {
        return accountHistoryRepository.CountChangesAsync(getChangesRequest, cancellationToken);
    }

    public async Task<List<ChangeEvent>> GetChangesAsync(GetChangesRequest getChangesRequest, CancellationToken cancellationToken)
    {
        var changes = await accountHistoryRepository.GetChangesAsync(getChangesRequest, cancellationToken);

        return MapToDomain(changes);
    }

    private static List<ChangeEvent> MapToDomain(List<ChangeEvent> changes)
    {
        return changes.Select(x =>
        {
            var mappedField = FieldMappings.TryGetValue(x.Field, out var domainField)
                ? domainField
                : x.Field;

            return new ChangeEvent
            {
                EntityId = x.EntityId,
                Timestamp = x.Timestamp,
                Field = mappedField,
                OldValue = MapValue(x.Field, x.OldValue),
                NewValue = MapValue(x.Field, x.NewValue)
            };
        }).ToList();
    }

    private static string? MapValue(string field, string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return value;
        }

        return field switch
        {
            nameof(AccountEntity.AccountTypeId) => Enum.TryParse<AccountType>(value, out var type) ? type.ToString() : value,
            nameof(AccountEntity.AccountStatusId) => Enum.TryParse<AccountStatus>(value, out var status) ? status.ToString() : value,
            _ => value
        };
    }
}
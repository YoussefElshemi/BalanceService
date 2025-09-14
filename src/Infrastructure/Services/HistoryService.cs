using Core.Interfaces;
using Core.Models;

namespace Infrastructure.Services;

public abstract class HistoryService<TEntity, TModel>(
    IHistoryRepository<TEntity, TModel> repository) : IHistoryService<TModel>
    where TEntity : class
    where TModel : class
{
    protected abstract Dictionary<string, string> FieldMappings { get; }
    protected abstract Dictionary<string, Func<string?, string?>> ValueMappers { get; }

    public Task<int> CountChangesAsync(GetChangesRequest getChangesRequest, CancellationToken cancellationToken)
    {
        return repository.CountChangesAsync(getChangesRequest, cancellationToken);
    }

    public async Task<List<ChangeEvent>> GetChangesAsync(GetChangesRequest getChangesRequest, CancellationToken cancellationToken)
    {
        var changes = await repository.GetChangesAsync(getChangesRequest, cancellationToken);
        return MapToDomain(changes);
    }

    private List<ChangeEvent> MapToDomain(List<ChangeEvent> changes)
    {
        return changes.Select(x =>
        {
            var mappedField = FieldMappings.TryGetValue(x.Field, out var domainField)
                ? domainField
                : x.Field;

            var mapValue = ValueMappers.TryGetValue(x.Field, out var mapper)
                ? mapper
                : v => v;

            return new ChangeEvent
            {
                EntityId = x.EntityId,
                Timestamp = x.Timestamp,
                Field = mappedField,
                OldValue = mapValue(x.OldValue),
                NewValue = mapValue(x.NewValue)
            };
        }).ToList();
    }
}
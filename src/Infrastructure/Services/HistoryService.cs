using Core.Interfaces;
using Core.Models;
using Core.ValueObjects;

namespace Infrastructure.Services;

public abstract class HistoryService<TEntity, TModel>(
    IHistoryRepository<TEntity, TModel> repository) : IHistoryService<TModel>
    where TEntity : class
    where TModel : class
{
    protected abstract Dictionary<ChangeEventField, ChangeEventField> FieldMappings { get; }
    protected abstract Dictionary<string, Func<ChangeEventValue?, ChangeEventValue?>> ValueMappers { get; }

    public Task<int> CountChangesAsync(GetChangesRequest getChangesRequest, CancellationToken cancellationToken)
    {
        return repository.CountChangesAsync(getChangesRequest, cancellationToken);
    }

    public async Task<List<ChangeEvent>> GetChangesAsync(GetChangesRequest getChangesRequest, CancellationToken cancellationToken)
    {
        var changes = await repository.GetChangesAsync(getChangesRequest, cancellationToken);
        return MapToDomain(changes);
    }

    private List<ChangeEvent> MapToDomain(List<ChangeEvent> changeEvents)
    {
        return changeEvents.Select(changeEvent =>
        {
            var mappedField = FieldMappings.TryGetValue(changeEvent.Field, out var domainField)
                ? domainField
                : changeEvent.Field;

            var mapValue = ValueMappers.TryGetValue(changeEvent.Field, out var mapper)
                ? mapper
                : v => v;

            return changeEvent with
            {
                Field = mappedField,
                OldValue = mapValue(changeEvent.OldValue),
                NewValue = mapValue(changeEvent.NewValue)
            };
        }).ToList();
    }
}
namespace Core.Models;

public interface IHistory<TKey, TDto>
    where TKey : IEquatable<TKey>
    where TDto : class
{
    public TKey GetPrimaryKey();
    public TDto ToDto();
}
namespace Core.Models;

public interface IHistory<TDto>
    where TDto : class
{
    public Guid GetPrimaryKey();
    public TDto ToDto();
}
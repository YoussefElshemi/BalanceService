using Core.Models;
using Presentation.Models;

namespace Presentation.Mappers;

public static class PagedResultsMapper
{
    public static PagedResultsDto<TDto> ToDto<TDto, TModel>(
        this PagedResults<TModel> results,
        Func<TModel, TDto> mapFunc)
    {
        return new PagedResultsDto<TDto>
        {
            Data = results.Data.Select(mapFunc).ToList(),
            MetaData = new PagedMetadataDto
            {
                TotalRecords = results.MetaData.TotalRecords,
                TotalPages = results.MetaData.TotalPages,
                PageSize = results.MetaData.PageSize,
                PageNumber = results.MetaData.PageNumber
            }
        };
    }
}
using Application.Queries.Dtos;
using Application.Utilities;

namespace Application.Queries;

public interface ICategoryQueries
{
    IEnumerable<CategoryDto> GetCategories(Guid userId);
    Option<CategoryDto> GetCategory(Guid id);
}
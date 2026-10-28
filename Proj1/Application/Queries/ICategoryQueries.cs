using Application.Queries.Dtos;
using Application.Utilities;
namespace Application.Queries;
public interface ICategoryQueries
{
    IEnumerable<CategoryDto> GetCategories();
    Option<CategoryDto> GetCategory(Guid id);
}
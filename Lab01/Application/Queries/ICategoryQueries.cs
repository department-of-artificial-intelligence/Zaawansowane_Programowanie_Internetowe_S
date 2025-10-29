using Contacts.Application.Queries.DTOs;

namespace Contacts.Application.Queries;

public interface ICategoryQueries
{
    public IEnumerable<CategoryDTO> GetCategories();

    public CategoryDTO? GetCategory(int categoryId);
}

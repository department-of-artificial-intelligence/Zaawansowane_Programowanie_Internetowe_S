using Contacts.Application.Queries.DTOs;

namespace Contacts.Application.Queries
{
    public interface ICategoryQueries
    {
        IEnumerable<CategoryDTO> GetCategories();

        CategoryDTO? GetCategory(int categoryId);
    }
}
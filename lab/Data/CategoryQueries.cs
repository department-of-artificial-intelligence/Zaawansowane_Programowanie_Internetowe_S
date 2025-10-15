using Contacts.Application.Queries;
using Contacts.Application.Queries.DTOs;
using Contacts.Data;

namespace Contacts.Data;

public class CategoriesQueries(AppDbContext context) : ICategoryQueries
{
    public IEnumerable<CategoryDTO> GetCategories()
    {
        return context.Categories
            .ToList()
            .Select(category => (CategoryDTO)category);
    }

    public CategoryDTO? GetCategory(int categoryId)
    {
        var category = context.Categories.Find(categoryId);
        return category is null ? null : (CategoryDTO)category;
    }
}

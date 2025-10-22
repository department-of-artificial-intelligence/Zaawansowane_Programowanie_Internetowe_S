using Contacts.Application.Queries;
using Contacts.Application.Queries.DTOs;

namespace Contacts.Data;

public class CategoriesQueries(AppDbContext context) : ICategoryQueries
{
    public IEnumerable<CategoryDTO> GetCategories()
    {
        return context.Categories.ToList().Select(category => (CategoryDTO)category);
    }

    public CategoryDTO? GetCategory(int categoryId)
    {
        return (CategoryDTO?)context.Categories.Find(categoryId);
    }
}
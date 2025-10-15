using Application.Queries;
using Application.Queries.DTOs;
using Data;

namespace Contacts.Data;

public class CategoryQueries(AppDbContext context) : ICategoryQueries
{
    public IEnumerable<CategoryDTO> GetCategories()
    {
        return context.Categories.ToList().Select(
            category => new CategoryDTO(category));
    }

    public CategoryDTO? GetCategory(int categoryId)
    {
        return new CategoryDTO(context.Categories.Find(categoryId));
    }
}

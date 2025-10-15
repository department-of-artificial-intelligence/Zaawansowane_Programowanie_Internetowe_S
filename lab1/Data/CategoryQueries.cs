using Contacts.Application.Queries;
using Contacts.Application.Queries.DTOs;
using Contacts.Application.UseCases.DTOs;
namespace Contacts.Data;

public class CategoriesQueries(AppDbContext context) : ICategoryQueries
{
    public IEnumerable<CategoryDTO> GetCategories()
    {
        return context.Categories.ToList().Select(category => (CategoryDTO)category);
    }
    public CategoryDTO? GetCategory(int categoryId)
    {
        return context.Categories.Find(categoryId);
    }
}
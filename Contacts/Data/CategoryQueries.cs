using Contacts.Application.Queries;
using Contacts.Application.UseCases.DTOs; 

namespace Contacts.Data;

public class CategoriesQueries(AppDbContext context) : ICategoryQueries
{
    public IEnumerable<CategoryDTO> GetCategories()
    {
        return context.Categories.ToList().Select(
            category => (CategoryDTO)category); 
    }

    public CategoryDTO? GetCategory(int categoryId)
    {
        var category = context.Categories.Find(categoryId);

        if (category == null)
        {
            return null;
        }

        return (CategoryDTO)category; 
    }
}
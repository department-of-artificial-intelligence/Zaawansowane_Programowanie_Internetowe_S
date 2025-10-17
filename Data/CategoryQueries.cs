using Contacts.Application.Domain;
using Contacts.Application.UseCases.DTOs;
using Data;
namespace Contacts.Data;

public class CategoriesQueries(AppDbContext context) : ICategoryQueries
{
    public IEnumerable<Category> GetCategories()
    {
        return context.Categories.ToList();
    }

    public Category? GetCategory(int categoryId)
    {
        return context.Categories.Find(categoryId);
    }
}
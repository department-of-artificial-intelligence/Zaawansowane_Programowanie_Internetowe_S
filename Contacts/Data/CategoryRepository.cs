using Contacts.Application.Domain;
using Contacts.Application.Repositories;
namespace Contacts.Data;
public class CategoryRepository(AppDbContext context) : ICategoryRepository
{
    public void Add(Category category)
    {
        context.Categories.Add(category);
    }
    public IEnumerable<Category> GetCategories()
        => context.Categories;
    public Category? GetCategory(int categoryId)
        => context.Categories.Find(categoryId);
    public Category? GetCategoryByName(string categoryName)
        => context.Categories.SingleOrDefault(
    category => category.Name == categoryName);
    public void RemoveCategory(Category category)
        => context.Remove(category);
    public bool IsCategoryInUse(int categoryId)
        => context.Emails.Any(e => e.CategoryId == categoryId)
           || context.Phones.Any(p => p.CategoryId == categoryId)
           || context.ImportantDates.Any(d => d.CategoryId == categoryId);
}
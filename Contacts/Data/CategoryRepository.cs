using Contacts.Application.Domain;
using Contacts.Application.Repositories;

namespace Contacts.Data;

public class CategoryRepository(AppDbContext context) : ICategoryRepository
{
    public void Add(Category category)
    {
        context.Categories.Add(category);
    }

    public void Edit(Category category){
        context.Categories.Update(category);
    }

    public IEnumerable<Category> GetCategories() => context.Categories;

    public Category? GetCategory(int categoryId) => context.Categories.Find(categoryId);

    public Category? GetCategoryByName(string categoryName) =>
        context.Categories.SingleOrDefault(category => category.Name == categoryName);


     public Category? GetCategoryByNameAndUser(string categoryName, int userId) =>
        context.Categories.SingleOrDefault(category => category.Name == categoryName && category.UserId == userId);

    public void RemoveCategory(Category category) => context.Remove(category);
}

using Application.Domain;

namespace Application.Repositories;

public interface ICategoryService
{
    IEnumerable<Category> GetCategories();
    Category? GetCategory(int categoryId);
    Category? GetCategoryByName(string categoryName);
    void Add(Category category);
    void RemoveCategory(Category category);
}
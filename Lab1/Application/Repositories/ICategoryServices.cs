using System.Runtime.CompilerServices;
using Contacts.Application.Domain;
namespace Contacts.Application.Repositories;

public interface ICategoryRepository
{
    IEnumerable<Category> GetCategories();
    Category? GetCategory(int categoryId);
    Category? GetCategoryByName(string categoryName);
    void Add(Category category);
    void RemoveCategory(Category categoryd);
}
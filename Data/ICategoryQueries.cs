using Contacts.Application.Domain;

namespace Data;

public interface ICategoryQueries
{
    IEnumerable<Category> GetCategories();

    Category? GetCategory(int categoryId);
}
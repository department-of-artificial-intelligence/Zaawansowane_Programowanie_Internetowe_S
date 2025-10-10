using Contacts.Application.Domain;

namespace Contacts.Application.Repository;
{
    IEnumerable<Category> GetCategories();
    Category? GetCategory(int categoryId);
    Category? GetCategoryByName(string categoryName);
    void Add(Category category);
    void RemoveCategory(Category category);
    
}


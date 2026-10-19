using Application.Domain;
using Application.Utilities;

namespace Application.Repositories;

public interface ICategoryRepository
{
    IEnumerable<Category> GetAll();
    Option<Category> GetById(Guid categoryId);
    Option<Category> GetByName(string categoryName);
    bool IsUsed(Guid categoryId);
    void Add(Category category);
    void Remove(Category category);
}
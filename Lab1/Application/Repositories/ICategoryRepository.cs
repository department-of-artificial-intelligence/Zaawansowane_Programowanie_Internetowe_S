using Application.Domain;
using Application.Utilities;
namespace Application.Repositories;

public interface ICategoryRepository
{
    IEnumerable<Category> GetAll();
    IEnumerable<Category> GetByUser(Guid userId);
    Option<Category> GetById(Guid categoryId);
    Option<Category> GetByNameAndUser(string categoryName, Guid userId);
    void Add(Category category);
    void Remove(Category category);
}

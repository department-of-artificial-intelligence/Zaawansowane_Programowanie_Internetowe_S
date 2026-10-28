using Application.Domain;
using Application.Repositories;
using Application.Utilities;

namespace Data;

public class CategoryRepository(AppDbContext context) : ICategoryRepository
{
    public void Add(Category category)
    {
        context.Categories.Add(category);
    }

    public IEnumerable<Category> GetAll() => context.Categories;
    public Option<Category> GetById(Guid categoryId)
    {
        var category = context.Categories.Find(categoryId);
        return category is not null
            ? Option.Some(category)
            : Option.None<Category>();
    }

    public Option<Category> GetByName(string categoryName)
    {
        var category =
            context
                .Categories
                .SingleOrDefault(category => category.Name == categoryName);
        return category is not null
            ? Option.Some(category)
            : Option.None<Category>();
    }
    public Option<Category> GetByNameAndUser(string categoryName, Guid userId)
    {
        var category = context.Categories
            .SingleOrDefault(category =>
                category.Name == categoryName &&
                category.UserId == userId);

        return category is not null
            ? Option.Some(category)
            : Option.None<Category>();
    }
    public void Remove(Category category)
    {
        context.Remove(category);
    }
}
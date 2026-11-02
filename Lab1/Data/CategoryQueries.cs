using Application.Queries;
using Application.Queries.Dtos;
using Application.Utilities;
namespace Data;

public class CategoryQueries(AppDbContext context) : ICategoryQueries
{
    public IEnumerable<CategoryDto> GetCategories(Guid userId) =>
        context.Categories
            .Where(category => category.UserId == userId)
            .ToList()
            .Select(CategoryDto.FromDomain);

    // Zadanie 9
    public Option<CategoryDto> GetCategory(Guid id, Guid userId)
    {
        var category = context.Categories
            .SingleOrDefault(category => category.Id == id && category.UserId == userId);
        return category is not null
            ? Option.Some(CategoryDto.FromDomain(category))
            : Option.None<CategoryDto>();
    }

    // Zadanie 11
    public Option<Guid> GetCategoryOwner(Guid id)
    {
        var owner = context.Categories
            .Where(category => category.Id == id)
            .Select(category => (Guid?)category.UserId)
            .SingleOrDefault();
        return owner is { } ownerId
            ? Option.Some(ownerId)
            : Option.None<Guid>();
    }
}

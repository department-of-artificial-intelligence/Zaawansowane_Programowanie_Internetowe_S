using Application.Queries;
using Application.Queries.Dtos;
using Application.Utilities;

namespace Data;

public class CategoryQueries(AppDbContext context) : ICategoryQueries
{
    public IEnumerable<CategoryDto> GetCategories(Guid userId)
        => context.Categories
            .Where(category => category.UserId == userId)
            .ToList()
            .Select(CategoryDto.FromDomain);

    public Option<CategoryDto> GetCategory(Guid id, Guid userId)
    {
        var category = context.Categories.SingleOrDefault(
            category => category.Id == id && category.UserId == userId);
        return category is not null
            ? Option.Some(CategoryDto.FromDomain(category))
            : Option.None<CategoryDto>();
    }

    public Option<CategoryDto> FindCategory(Guid id)
    {
        var category = context.Categories.Find(id);
        return category is not null
            ? Option.Some(CategoryDto.FromDomain(category))
            : Option.None<CategoryDto>();
    }
}

using Application.Domain;

namespace Application.Queries.Dtos;

public class CategoryDto
{
    public Guid Id { get; }
    public string Name { get; }
    public Guid UserId { get; }

    private CategoryDto(Guid id, string name, Guid userId)
    {
        Id = id;
        Name = name;
        UserId = userId;
    }

    public static CategoryDto FromDomain(Category category)
        => new (category.Id, category.Name, category.UserId);
}

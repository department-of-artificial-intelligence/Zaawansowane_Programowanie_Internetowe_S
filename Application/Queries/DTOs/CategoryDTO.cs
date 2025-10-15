using Contacts.Application.Domain;

namespace Contacts.Application.Queries.DTOs;

public class CategoryDTO
{
    public int Id { get; init; }
    public string Name { get; init; } = string.Empty;

    public static implicit operator CategoryDTO(Category category)
        => new CategoryDTO { Id = category.Id, Name = category.Name };
}

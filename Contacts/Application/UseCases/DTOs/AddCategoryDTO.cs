using Contacts.Application.Domain;

namespace Contacts.Application.UseCases.DTOs;

public class AddCategoryDTO
{
    public string Name { get; set; } = string.Empty;

    public static implicit operator Category(AddCategoryDTO dto) => new(dto.Name);
}


public class EditCategoryDTO
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;

    public static implicit operator Category(EditCategoryDTO dto) => new(dto.Name);
}

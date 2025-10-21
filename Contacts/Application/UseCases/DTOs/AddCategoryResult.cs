using Contacts.Application.Domain;

namespace Contacts.Application.UseCases.DTOs;

public class AddCategoryResult
{
    public int Id { get; init; }
    public string Name { get; init; } = string.Empty;

    public static implicit operator AddCategoryResult(Category category) =>
        new AddCategoryResult { Id = category.Id, Name = category.Name };

    public static implicit operator AddCategoryResult(AddCategoryDTO category) =>
        new AddCategoryResult { Name = category.Name };
}


public class EditCategoryResult
{
    public int Id { get; set; }
    public string Name { get; init; } = string.Empty;

    public static implicit operator EditCategoryResult(Category category) =>
        new EditCategoryResult { Id = category.Id, Name = category.Name };

    public static implicit operator EditCategoryResult(EditCategoryDTO category) =>
        new EditCategoryResult { Id = category.Id, Name = category.Name };
}

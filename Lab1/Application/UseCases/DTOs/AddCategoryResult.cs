using Contacts.Application.Domain;

namespace Contacts.Application.UseCases.DTOs;

public class AddCategoryResult
{
    public int Id {get; init;}
    public string Name {get; init;} = string.Empty;

    public static implicit operator AddCategoryResult(Category category)
        => new AddCategoryResult{Id = category.Id, Name = category.Name};
}
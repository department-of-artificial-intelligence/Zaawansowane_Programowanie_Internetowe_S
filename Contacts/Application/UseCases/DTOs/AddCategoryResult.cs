using System.Reflection.Metadata;
using Contacts.Application.Domain;

namespace Contacts.Application.UseCases.DTOs;

public class AddCategoryResult
{
    public int id { get; init; }
    public string Name { get; init; } = string.Empty;

    public static implicit operator AddCategoryResult(Category category) => new AddCategoryResult { id = category.Id, Name = category.Name };
}
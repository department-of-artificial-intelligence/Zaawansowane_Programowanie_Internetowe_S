using Application.Domain;
using Application.Utilities;

namespace Application.UseCases.Dtos;

public class AddCategoryDto(string name)
{
    public string Name { get; } = name;

    public Result<Category> ToDomain() => Category.Create(Name);
}
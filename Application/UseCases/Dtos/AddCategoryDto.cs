using Application.Domain;
using Application.Utilities;
namespace Application.UseCases.Dtos;

public class AddCategoryDto(string name, Guid userId)
{
    public string Name { get; } = name;
    public Guid UserId { get; } = userId;
    public Result<Category> ToDomain() => Category.Create(Name, UserId);
}
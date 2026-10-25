using Application.Domain;

namespace Application.UseCases.Dtos;

public class AddCategoryResult(Guid id, string name)
{
    public Guid Id { get; } = id;
    public string Name { get; } = name;

    internal static AddCategoryResult FromDomain(Category category) =>
        new(category.Id, category.Name);
}
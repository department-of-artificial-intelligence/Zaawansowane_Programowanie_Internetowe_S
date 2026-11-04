namespace Application.UseCases.Dtos;

public class RenameCategoryDto(Guid id, string name)
{
    public Guid Id { get; } = id;
    public string Name { get; } = name;
}
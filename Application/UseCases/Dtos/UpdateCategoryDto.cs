namespace Application.UseCases.Dtos;

public class UpdateCategoryDto(Guid id, string newName)
{
    public Guid Id { get; } = id;
    public string NewName { get; } = newName;
}
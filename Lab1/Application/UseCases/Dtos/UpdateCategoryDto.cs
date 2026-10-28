namespace Application.UseCases.Dtos;

public class UpdateCategoryDto(Guid id, string name, Guid userId) {
	public Guid Id { get; } = id;
	public Guid UserId { get; } = userId;
	public string Name { get; } = name;
}

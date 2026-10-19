namespace Application.UseCases.Dtos;

public class RemoveImportantDateDto(Guid contactId, string name)
{
    public Guid ContactId { get; } = contactId;
    public string Name { get; } = name;
}
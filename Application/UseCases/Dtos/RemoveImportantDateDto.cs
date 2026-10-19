namespace Application.UseCases.Dtos;

public class RemoveImportantDateDto(Guid contactId, string label)
{
    public Guid ContactId { get; } = contactId;
    public string Label { get; } = label;
}
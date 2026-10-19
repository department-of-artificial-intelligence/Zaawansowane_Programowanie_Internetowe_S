namespace Application.UseCases.Dtos;

public class RemoveImportantDateDto(Guid contactId, string description, DateOnly date)
{
    public Guid ContactId { get; } = contactId;
    public string Description { get; } = description;
    public DateOnly Date { get; } = date;
}
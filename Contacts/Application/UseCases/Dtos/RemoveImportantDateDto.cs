namespace Application.UseCases.Dtos;

public class RemoveImportantDateDto(Guid contactId, DateOnly date, string description)
{
    public Guid ContactId { get; } = contactId;
    public DateOnly Date {get; } = date;
    public string Description {get; } = description;
}
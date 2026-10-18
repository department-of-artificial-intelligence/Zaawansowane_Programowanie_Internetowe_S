namespace Application.UseCases.Dtos;

public class AddImportantDateDto(Guid contactId, DateOnly date, string description)
{
    public Guid ContactId { get; } = contactId;
    public DateOnly Date {get; } = date;
    public string Description {get; } = description;
}
namespace Application.UseCases.Dtos;

public class AddImportantDateDto(Guid contactId, DateOnly date, string label)
{
    public Guid ContactId { get; } = contactId;
    public DateOnly Date { get; } = date;
    public string Label { get; } = label;
}
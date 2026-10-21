namespace Application.UseCases.Dtos;

public class AddImportantDateDto(Guid contactId, string name, DateTime date)
{
    public Guid ContactId { get; } = contactId;
    public string Name { get; } = name;
    public DateTime Date { get; } = date;
}
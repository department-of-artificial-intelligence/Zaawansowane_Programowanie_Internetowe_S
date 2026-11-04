namespace Application.UseCases.Dtos;

public class ImportantDateDto(Guid contactId, string name, DateOnly date)
{
    public Guid ContactId { get; } = contactId;
    public string Name { get; } = name;
    public DateOnly Date { get; } = date;
}

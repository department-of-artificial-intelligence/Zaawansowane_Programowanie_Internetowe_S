namespace Contacts.Application.UseCases.DTOs;

public class AddImportantDateDTO
{
    public int ContactId { get; init; }
    public int CategoryId { get; init; }
    public DateOnly Date { get; init; }
    public string Description { get; init; } = string.Empty;
}
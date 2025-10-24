namespace Contacts.Application.UseCases.DTOs;

public class AddImportantDateResult
{
    public bool Success { get; init; }
    public int ImportantDateId { get; init; }
    public string ErrorMessage { get; init; } = string.Empty;
}
using Contacts.Application.Domain;

namespace Contacts.Application.UseCases.DTOs;

public class UpdateContactDTO
{
    public int Id { get; init; }
    public string Firstname { get; init; } = string.Empty;
    public string LastName { get; init; } = string.Empty;
    public Sex Sex { get; init; }
    public int Age { get; init; }
}
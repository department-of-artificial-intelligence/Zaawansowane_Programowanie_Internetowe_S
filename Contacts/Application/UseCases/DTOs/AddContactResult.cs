using Contacts.Application.Domain;

namespace Contacts.Application.UseCases.DTOs;

public class AddContactResult
{
    public int Id { get; init; }
    public string Firstname { get; init; } = string.Empty;
    public string LastName { get; init; } = string.Empty;
    public Sex Sex { get; init; }
    public int Age { get; init; }

    public static implicit operator AddContactResult(Contact contact)
        => new AddContactResult
        {
            Id = contact.Id,
            Firstname = contact.Firstname,
            LastName = contact.LastName,
            Sex = contact.Sex,
            Age = contact.Age.Value
        };
}
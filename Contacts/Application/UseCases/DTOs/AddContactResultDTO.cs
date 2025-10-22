using Contacts.Application.Domain;

namespace Contacts.Application.UseCases.DTOs;

public class AddContactResult
{
    public int Id { get; init; }
    public string FirstName { get; init; } = string.Empty;
    public string LastName { get; init; } = string.Empty;
    public Sex Sex { get; init; }
    public ICollection<Email> Emails { get; init; } = null!;
    public Age Age { get; init; } = null!;

    public static implicit operator AddContactResult(Contact Contact) =>
        new AddContactResult { Id = Contact.Id, FirstName = Contact.FirstName, Sex = Contact.Sex, Emails = Contact.Emails, Age = Contact.Age };

    public static implicit operator AddContactResult(AddContactDTO Contact) =>
        new AddContactResult { Id = Contact.Id, FirstName = Contact.FirstName, Sex = Contact.Sex, Emails = Contact.Emails, Age = Contact.Age };
}


public class EditContactResult
{
    public int Id { get; init; }
    public string FirstName { get; init; } = string.Empty;
    public string LastName { get; init; } = string.Empty;
    public Sex Sex { get; init; }
    public ICollection<Email> Emails { get; init; } = null!;
    public Age Age { get; init; } = null!;

    public static implicit operator EditContactResult(Contact Contact) =>
        new EditContactResult { Id = Contact.Id, FirstName = Contact.FirstName, Sex = Contact.Sex, Emails = Contact.Emails, Age = Contact.Age };

    public static implicit operator EditContactResult(EditContactDTO Contact) =>
        new EditContactResult { Id = Contact.Id, FirstName = Contact.FirstName, Sex = Contact.Sex, Emails = Contact.Emails, Age = Contact.Age };
}

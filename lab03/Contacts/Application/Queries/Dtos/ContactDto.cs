using Application.Domain;

namespace Application.Queries.Dtos;

public class ContactDto
{
    public Guid Id { get; }
    public string FirstName { get; }
    public string LastName { get; }
    public Sex Sex { get; }
    public int Age { get; }
    public IEnumerable<ContactEmailDto> Emails { get; }
    public IEnumerable<string> Phones { get; }
    public IEnumerable<ContactImportantDateDto> ImportantDates { get; }

    private ContactDto(
        Guid id, string firstName, string lastName, Sex sex, int age,
        IEnumerable<ContactEmailDto> emails,
        IEnumerable<string> phones,
        IEnumerable<ContactImportantDateDto> importantDates)
    {
        Id = id;
        FirstName = firstName;
        LastName = lastName;
        Sex = sex;
        Age = age;
        Emails = emails;
        Phones = phones;
        ImportantDates = importantDates;
    }

    public static ContactDto FromDomain(Contact contact)
        => new(contact.Id, contact.FirstName.Value, contact.LastName.Value,
            contact.Sex, contact.Age.Value,
            contact.Emails.Select(e => new ContactEmailDto(e.Address.Value, e.CategoryId)).ToList(),
            contact.Phones.Select(p => p.Value).ToList(),
            contact.ImportantDates.Select(d => new ContactImportantDateDto(d.Date, d.Description)).ToList());
}

public record ContactEmailDto(string Address, Guid CategoryId);
public record ContactImportantDateDto(DateOnly Date, string Description);

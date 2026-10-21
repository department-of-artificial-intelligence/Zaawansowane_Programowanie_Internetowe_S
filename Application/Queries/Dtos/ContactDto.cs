using Application.Domain;

namespace Application.Queries.Dtos;

public class ContactDto
{
    public Guid Id { get; }
    public string FirstName { get; }
    public string LastName { get; }
    public Sex Sex { get; }
    public int Age { get; }
    public IReadOnlyList<ContactEmailDto> Emails { get; }
    public IReadOnlyList<ContactPhoneDto> Phones { get; }
    public IReadOnlyList<ContactImportantDateDto> ImportantDates { get; }

    private ContactDto(
        Guid id,
        string firstName,
        string lastName,
        Sex sex,
        int age,
        IReadOnlyList<ContactEmailDto> emails,
        IReadOnlyList<ContactPhoneDto> phones,
        IReadOnlyList<ContactImportantDateDto> importantDates)
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

    public static ContactDto FromDomain(Contact contact) =>
        new(contact.Id,
            contact.FirstName.Value,
            contact.LastName.Value,
            contact.Sex,
            contact.Age.Value,
            contact.Emails
                .Select(email => new ContactEmailDto(email.Address.Value, email.CategoryId))
                .ToList(),
            contact.Phones
                .Select(phone => new ContactPhoneDto(phone.Number.Value, phone.Type))
                .ToList(),
            contact.ImportantDates
                .Select(date => new ContactImportantDateDto(date.Date, date.Label.Value))
                .ToList());
}

public record ContactEmailDto(string Address, Guid CategoryId);

public record ContactPhoneDto(string Number, PhoneType Type);

public record ContactImportantDateDto(DateOnly Date, string Label);
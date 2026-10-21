using Application.Domain;

namespace Application.Queries.Dtos;

public class ContactDto
{
    public Guid Id { get; }
    public FirstName FirstName { get;}
    public LastName LastName { get; }
    public Sex Sex { get; }
    public Age Age { get; }
    public IReadOnlyCollection<Email> Emails { get; }
    public IReadOnlyCollection<Phone> Phones { get; }
    public IReadOnlyCollection<ImportantDate> ImportantDates { get; }
    private ContactDto(Guid id, FirstName firstName, LastName lastName, Sex sex, Age age, IReadOnlyCollection<Email> emails, IReadOnlyCollection<Phone> phones, IReadOnlyCollection<ImportantDate> importantDates)
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
        => new (contact.Id, contact.FirstName, contact.LastName, contact.Sex, contact.Age, contact.Emails, contact.Phones, contact.ImportantDates);
}

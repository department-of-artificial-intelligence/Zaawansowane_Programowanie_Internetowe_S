using Application.Domain;

namespace Application.Queries.Dtos;

public class ContactDto {
    public Guid Id { get; }
    public string FirstName { get; }
    public string LastName { get; }
    public Sex Sex { get; }
    public int Age { get; }
    public IEnumerable<EmailDto> Emails { get; }
    public IEnumerable<PhoneDto> Phones { get; }
    public IEnumerable<ImportantDateDto> ImportantDates { get; }

    private ContactDto(Guid id, string firstName, string lastName, Sex sex, int age,
                       IEnumerable<EmailDto> emails, IEnumerable<PhoneDto> phones, IEnumerable<ImportantDateDto> importantDates){
        Id = id;
        FirstName = firstName;
        LastName = lastName;
        Sex = sex;
        Age = age;
        Emails = emails;
        Phones = phones;
        ImportantDates = importantDates;
    }

    public static ContactDto FromDomain(Contact contact) => new(
        contact.Id,
        contact.FirstName.Value,
        contact.LastName.Value,
        contact.Sex,
        contact.Age.Value,
        contact.Emails.Select(EmailDto.FromDomain),
        contact.Phones.Select(PhoneDto.FromDomain),
        contact.ImportantDates.Select(ImportantDateDto.FromDomain)
    );
}

public class EmailDto {
    public string Address { get; }
    public Guid CategoryId { get; }

    private EmailDto(string address, Guid categoryId){
        Address = address;
        CategoryId = categoryId;
    }

    public static EmailDto FromDomain(Email email) => new(email.Address.ToString(), email.CategoryId);
}

public class PhoneDto {
    public string Number { get; }
    public Guid CategoryId { get; }

    private PhoneDto(string number, Guid categoryId){
        Number = number;
        CategoryId = categoryId;
    }

    public static PhoneDto FromDomain(Phone phone) => new(phone.Number.ToString(), phone.CategoryId);
}

public class ImportantDateDto {
    public string Description { get; }
    public DateOnly Date { get; }

    private ImportantDateDto(string description, DateOnly date){
        Description = description;
        Date = date;
    }

    public static ImportantDateDto FromDomain(ImportantDate date) => new(date.Description, date.Date);
}

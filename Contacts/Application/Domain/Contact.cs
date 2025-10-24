using System.Dynamic;

namespace Contacts.Application.Domain;

public class Contact
{
    public int Id { get; private set; }
    public string Firstname { get; private set; } = string.Empty;
    public string LastName { get; private set; } = string.Empty;
    public Sex Sex { get; private set; }
    public ICollection<Email> Emails { get; private set; } = null!;
    public ICollection<Phone> Phones { get; private set; } = new List<Phone>();
    public ICollection<ImportantDate> Dates { get; private set; } = new List<ImportantDate>();
    public Age Age { get; private set; } = null!;

    private Contact() { }

    public Contact(int id, string firstName, string lastName, Sex sex, ICollection<Email> emails, Age age)
    {
        Id = id;
        Firstname = string.IsNullOrWhiteSpace(firstName) ? throw new ArgumentException(nameof(firstName)) : firstName;
        LastName = string.IsNullOrWhiteSpace(lastName) ? throw new ArgumentException(nameof(lastName)) : lastName;
        Sex = sex;
        Emails = emails ?? throw new ArgumentNullException(nameof(emails));
        Age = age ?? throw new ArgumentNullException(nameof(age));
    }
    public bool HasEmail(Email email)
    {
        return Emails.Any(e => e.Address == email.Address);
    }

    public void AddEmail(Email email)
    {
        if(!HasEmail(email))
        {
            Emails.Add(email);
        }
    }

    public bool HasPhone(Phone phone)
    {
        return Phones.Any(p => p.Number == phone.Number);
    }

    public void AddPhone(Phone phone)
    {
        if(!HasPhone(phone))
        {
            Phones.Add(phone);
        }
    }

    public bool HasDate(ImportantDate date)
    {
        return Dates.Any(d => d.Info.Date == date.Info.Date && string.Equals(d.Info.Description, date.Info.Description, StringComparison.OrdinalIgnoreCase));
    }

    public void AddDate(ImportantDate date)
    {
        if(!HasDate(date))
        {
            Dates.Add(date);
        }
    }

    public void UpdateFirstname(string firstname)
    {
        Firstname = string.IsNullOrWhiteSpace(firstname) ? throw new ArgumentException(nameof(firstname)) : firstname;
    }

    public void UpdateLastName(string lastName)
    {
        LastName = string.IsNullOrWhiteSpace(lastName) ? throw new ArgumentException(nameof(lastName)) : lastName;
    }

    public void UpdateSex(Sex sex)
    {
        Sex = sex;
    }

    public void UpdateAge(Age age)
    {
        Age = age ?? throw new ArgumentNullException(nameof(age));
    }
}
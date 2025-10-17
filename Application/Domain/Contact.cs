using Application.Domain;

namespace Contacts.Application.Domain;

public class Contact
{
    public int Id { get; private set; }
    public FirstName FirstName { get; private set; } = null!;
    public LastName LastName { get; private set; } = null!;
    public Sex Sex { get; private set; }
    public ICollection<Email> Emails { get; private set; } = null!;
    public Age Age { get; private set; } = null!;
    private Contact() { }
    public Contact(int id, FirstName firstName, LastName lastName,
    Sex sex, ICollection<Email> emails, Age age)
    {
        Id = id;
        FirstName = firstName
        ?? throw new ArgumentException(nameof(firstName));
        LastName = lastName
        ?? throw new ArgumentException(nameof(lastName));
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
        if (!HasEmail(email))
        {
            Emails.Add(email);
        }
    }
}
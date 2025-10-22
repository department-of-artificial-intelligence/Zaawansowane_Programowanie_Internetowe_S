namespace Contacts.Application.Domain;

public class Contact
{
    public int Id { get; set; }
    public FirstName FirstName { get; set; }
    public LastName LastName { get; set; }
    public Sex Sex { get; set; }

    public ICollection<Email> Emails { get; private set; } = null!;

    public Age Age { get; private set; } = null!;


    private Contact() { }


    public Contact(int id, FirstName firstName, LastName lastName, Sex sex, ICollection<Email> emails, Age age)
    {

        Id = id;
        FirstName = firstName ?? throw new ArgumentNullException(nameof(firstName));
        LastName = lastName ?? throw new ArgumentNullException(nameof(lastName));
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
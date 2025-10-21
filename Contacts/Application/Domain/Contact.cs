using System.ComponentModel.DataAnnotations;

namespace Contacts.Application.Domain;

public class Contact
{
    public int Id { get; private set; }

    [Required(ErrorMessage = "First name is required.")]
    [StringLength(50, ErrorMessage = "First name cannot exceed 50 characters.")]
    public string FirstName { get; private set; } = string.Empty;

    [Required(ErrorMessage = "Last name is required.")]
    [StringLength(50, ErrorMessage = "Last name cannot exceed 50 characters.")]
    public string LastName { get; private set; } = string.Empty;
    public Sex Sex { get; private set; }
    public ICollection<Email> Emails { get; private set; } = null!;
    public Age Age { get; private set; } = null!;

    private Contact() { }

    public Contact(
        int id,
        string firstName,
        string lastName,
        Sex sex,
        ICollection<Email> emails,
        Age age
    )
    {
        Id = id;
        FirstName = string.IsNullOrWhiteSpace(firstName)
            ? throw new ArgumentException(nameof(firstName))
            : firstName;
        LastName = string.IsNullOrWhiteSpace(lastName)
            ? throw new ArgumentException(nameof(lastName))
            : lastName;
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

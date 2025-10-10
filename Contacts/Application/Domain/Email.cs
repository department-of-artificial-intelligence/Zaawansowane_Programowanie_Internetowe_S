using System.ComponentModel;

namespace Contacts.Application.Domain;

public class Email
{
    public int Id { get; private set; }
    public EmailAddress Address { get; private set; } = null!;
    public Contacts Contact { get; private set; } = null!;
    public int ContactId { get; private set; }

    public int CategoryId { get; private set; }

    public Category Category { get; private set; } = null!;
    private Email()
    {

    }
    
    public Email(int id)
}
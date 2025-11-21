using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Contacts.Application.Domain
{
public class Email
{
    public int Id { get; private set; }
    public EmailAddress Address { get; private set; } = null!;
    
    public Contact Contact { get; private set; } = null!;
    public int ContactId { get; private set; }

    public int CategoryId { get; private set; }
    public Category Category { get; private set; } = null!;

    // Konstruktor dla EF Core
    private Email() { } 

    // POPRAWKA: Publiczny konstruktor do tworzenia NOWEGO emaila
    public Email(EmailAddress address, int categoryId)
    {
        Address = address;
        CategoryId = categoryId; // Ustawiamy tylko ID
    }
}
}
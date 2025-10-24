using Contacts.Application.Queries;
using Contacts.Application.Queries.DTOs;

namespace Contacts.Data;

public class ContactQueries(AppDbContext context) : IContactQueries
{
    public IEnumerable<ContactDTO> GetContacts()
    {
        return context.Contacts.ToList().Select(contact => (ContactDTO)contact);
    }

    public ContactDTO? GetContact(int contactId)
    {
        return (ContactDTO?)context.Contacts.Find(contactId);
    }
}

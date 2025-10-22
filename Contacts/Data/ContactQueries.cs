using Contacts.Application.Queries;
using Contacts.Application.Queries.DTOs;

namespace Contacts.Data;

public class ContactQueries(AppDbContext context) : IContactQueries
{
    public IEnumerable<ContactDTO> GetContacts()
    {
        return context.Contacts.ToList().Select(Contact => (ContactDTO)Contact);
    }

    public ContactDTO? GetContact(int ContactId)
    {
        return (ContactDTO?)context.Contacts.Find(ContactId);
    }
}

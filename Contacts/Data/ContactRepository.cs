using Application.Domain;
using Application.Repositories;
using Application.Utilities;
using Microsoft.EntityFrameworkCore;

namespace Data;

public class ContactRepository(AppDbContext context) : IContactRepository
{
    public void Add(Contact contact)
    {
        context.Contacts.Add(contact);
    }

    public IEnumerable<Contact> GetAll() =>
        context.Contacts
            .Include(c => c.Emails)
            .Include(c => c.Phones)
            .Include(c => c.ImportantDates);

    public Option<Contact> GetById(Guid contactId)
    {
        var contact = context.Contacts
            .Include(c => c.Emails)
            .Include(c => c.Phones)
            .Include(c => c.ImportantDates)
            .SingleOrDefault(c => c.Id == contactId);

        return contact is not null
            ? Option.Some(contact)
            : Option.None<Contact>();
    }

    public void Remove(Contact contact)
    {
        context.Remove(contact);
    }
}
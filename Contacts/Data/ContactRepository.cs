using System.Collections.Generic;
using System.Linq;
using Contacts.Application.Domain;
using Contacts.Application.Repositories;

namespace Contacts.Data;

public class ContactRepository(AppDbContext context) : IContactRepository
{
    public IEnumerable<Contact> GetContacts() => context.Contacts;
    public Contact? GetContact(int contactId) => context.Contacts.Find(contactId);

    public void Add(Contact contact)
    {
        context.Contacts.Add(contact);
    }

    public void Update(Contact contact)
    {
        context.Contacts.Update(contact);
    }

    public void Remove(Contact contact)
    {
        context.Contacts.Remove(contact);
    }
}
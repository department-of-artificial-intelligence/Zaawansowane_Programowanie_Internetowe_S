using Contacts.Application.Domain;
using Contacts.Application.Repositories;

namespace Contacts.Data;

public class ContactRepository(AppDbContext context) : IContactRepository
{
    public void Add(Contact contact)
    {
        context.Contacts.Add(contact);
    }

    public IEnumerable<Contact> GetContacts() => context.Contacts;

    public Contact? GetContact(int contactId) => context.Contacts.Find(contactId);

    public Contact? GetContactByName(string contactFirstName, string contactLastName) =>
        context.Contacts.SingleOrDefault(contact =>
            contact.FirstName == contactFirstName && contact.LastName == contactLastName
        );

    public void RemoveContact(Contact contact) => context.Remove(contact);

    public void UpdateContact(Contact contact)
    {
        context.Contacts.Update(contact);
    }
}

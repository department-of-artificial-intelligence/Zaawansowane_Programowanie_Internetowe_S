using Contacts.Application.Domain;
using Contacts.Application.Repositories;

namespace Contacts.Data;

public class ContactRepository(AppDbContext context) : IContactRepository
{
    public void Add(Contact contact)
    {
        context.Contacts.Add(contact);
    }

    public void Edit(Contact contact){
        context.Contacts.Update(contact);
    }

    public IEnumerable<Contact> GetContacts() => context.Contacts;

    public Contact? GetContact(int contactId) => context.Contacts.Find(contactId);

    public Contact? GetContactByName(string FirstName, string LastName) =>
        context.Contacts.SingleOrDefault(contact =>
            contact.FirstName == FirstName && contact.FirstName == LastName
        );

    public void RemoveContact(Contact contact) => context.Remove(contact);
}

using Contacts.Application.Domain;

namespace Contacts.Application.Repositories;

public interface IContactRepository
{
    IEnumerable<Contact> GetContacts();
    Contact? GetContact(int contactId);
    Contact? GetContactByName(string FirstName, string LastName);
    void Add(Contact Contact);
    void RemoveContact(Contact Contact);
    void Edit(Contact Contact);
}
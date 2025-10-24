using System.Collections.Generic;
using Contacts.Application.Domain;

namespace Contacts.Application.Repositories;

public interface IContactRepository
{
    IEnumerable<Contact> GetContacts();
    Contact? GetContact(int contactId);

    void Add(Contact contact);
    void Update(Contact contact);
    void Remove(Contact contact);
}
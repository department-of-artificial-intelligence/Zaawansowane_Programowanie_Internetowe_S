using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Contacts.Application.Domain;

namespace Contacts.Application.Repositories;

public interface IContactRepository
{
    IEnumerable<Contact> GetContacts();
    Contact? GetContact(int contactId);
    public Contact? GetContactByName(string contactFirstName, string contactLastName);
    void Add(Contact contact);
    void RemoveContact(Contact contact);
    void UpdateContact(Contact contact);
}

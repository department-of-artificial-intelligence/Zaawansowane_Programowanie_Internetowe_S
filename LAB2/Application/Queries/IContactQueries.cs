using Contacts.Application.Domain;
using Contacts.Application.Queries.DTOs;
using Contacts.Application.Repositories;

namespace Contacts.Application.Queries;

public interface IContactQueries
{
    public IEnumerable<ContactDTO> GetContacts();
    public ContactDTO? GetContact(int contactId);
}

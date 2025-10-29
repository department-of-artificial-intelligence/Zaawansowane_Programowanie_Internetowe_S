using Contacts.Application.Queries.DTOs;
namespace Contacts.Application.Queries;

public interface IContactQueries
{
    public IEnumerable<ContactDTO> GetContacts();

    public ContactDTO? GetContact(int ContactId);
}
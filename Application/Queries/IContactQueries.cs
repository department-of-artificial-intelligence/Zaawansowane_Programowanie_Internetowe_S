using Application.Queries.Dtos;
using Application.Utilities;

namespace Application.Queries;

public interface IContactQueries
{
    IEnumerable<ContactDto> GetContacts();
    Option<ContactDto> GetContact(Guid id);
}
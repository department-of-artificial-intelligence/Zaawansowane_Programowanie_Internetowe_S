using Application.Queries.Dtos;
using Application.Utilities;

namespace Application.Queries;

// Zadanie 8 - użytkownik widzi wyłącznie swoje kontakty.
public interface IContactQueries
{
    IEnumerable<ContactDto> GetContacts(Guid userId);
    Option<ContactDto> GetContact(Guid id, Guid userId);
}

using Application.Queries;
using Application.Queries.Dtos;
using Application.Utilities;

namespace Data;

public class ContactQueries(AppDbContext context) : IContactQueries
{
    public IEnumerable<ContactDto> GetContacts(Guid userId)
        => context.Contacts
            .Where(contact => contact.UserId == userId)
            .ToList()
            .Select(ContactDto.FromDomain);

    public Option<ContactDto> GetContact(Guid id, Guid userId)
    {
        var contact = context.Contacts.SingleOrDefault(
            contact => contact.Id == id && contact.UserId == userId);
        return contact is not null
            ? Option.Some(ContactDto.FromDomain(contact))
            : Option.None<ContactDto>();
    }
}

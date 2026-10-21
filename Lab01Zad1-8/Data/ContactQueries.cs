using Application.Queries;
using Application.Queries.Dtos;
using Application.Utilities;

namespace Data;

public class ContactQueries(AppDbContext context) : IContactQueries
{
    public IEnumerable<ContactDto> GetContacts()
        => context.Contacts.ToList().Select(ContactDto.FromDomain);
    public Option<ContactDto> GetContact(Guid id)
    {
        var contact = context.Contacts.Find(id);
        return contact is not null
            ? Option.Some(ContactDto.FromDomain(contact))
            : Option.None<ContactDto>();
    }
}

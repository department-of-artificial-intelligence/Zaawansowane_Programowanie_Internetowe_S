using Application.Queries;
using Application.Queries.Dtos;
using Application.Utilities;
using Microsoft.EntityFrameworkCore;

namespace Data;

public class ContactQueries(AppDbContext context) : IContactQueries
{
    public IEnumerable<ContactDto> GetContacts()
        => context.Contacts
            .AsNoTracking()
            .ToList()
            .Select(ContactDto.FromDomain);

    public Option<ContactDto> GetContact(Guid id)
    {
        var contact = context.Contacts
            .AsNoTracking()
            .SingleOrDefault(c => c.Id == id);

        return contact is not null
            ? Option.Some(ContactDto.FromDomain(contact))
            : Option.None<ContactDto>();
    }
}
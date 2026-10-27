using Application.Queries;
using Application.Queries.Dtos;
using Application.Utilities;
using Microsoft.EntityFrameworkCore;
namespace Data;

public class ContactQueries(AppDbContext context) : IContactQueries
{
    public IEnumerable<ContactSummaryDto> GetContacts() =>
        context.Contacts
            .AsNoTracking()
            .ToList()
            .Select(ContactSummaryDto.FromDomain);

    public Option<ContactDetailsDto> GetContact(Guid id)
    {
        // E-maile, telefony i daty to typy posiadane (OwnsMany),
        // więc EF wczytuje je razem z kontaktem.
        var contact = context.Contacts
            .AsNoTracking()
            .SingleOrDefault(c => c.Id == id);

        return contact is not null
            ? Option.Some(ContactDetailsDto.FromDomain(contact))
            : Option.None<ContactDetailsDto>();
    }
}

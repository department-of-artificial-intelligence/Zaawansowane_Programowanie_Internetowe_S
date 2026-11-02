using Application.Queries;
using Application.Queries.Dtos;
using Application.Utilities;
using Microsoft.EntityFrameworkCore;
namespace Data;

public class ContactQueries(AppDbContext context) : IContactQueries
{
    public IEnumerable<ContactSummaryDto> GetContacts(Guid userId) =>
        context.Contacts
            .AsNoTracking()
            .Where(c => c.UserId == userId)
            .ToList()
            .Select(ContactSummaryDto.FromDomain);

    public Option<ContactDetailsDto> GetContact(Guid id, Guid userId)
    {
        var contact = context.Contacts
            .AsNoTracking()
            .SingleOrDefault(c => c.Id == id && c.UserId == userId);

        return contact is not null
            ? Option.Some(ContactDetailsDto.FromDomain(contact))
            : Option.None<ContactDetailsDto>();
    }
}

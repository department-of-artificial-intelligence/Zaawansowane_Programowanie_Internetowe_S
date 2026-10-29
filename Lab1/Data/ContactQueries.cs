using Application.Queries;
using Application.Queries.Dtos;
using Application.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data
{
    public class ContactQueries(AppDbContext context) : IContactQueries
    {
        public IEnumerable<ContactDto> GetContacts(Guid userId) =>
            context.Contacts
                .Where(c => c.UserId == userId)
                .ToList()
                .Select(ContactDto.FromDomain);

        public Option<ContactDto> GetContact(Guid id, Guid userId)
        {
            var contact = context.Contacts
                .SingleOrDefault(c => c.Id == id && c.UserId == userId);

            return contact is not null
                ? Option.Some(ContactDto.FromDomain(contact))
                : Option.None<ContactDto>();
        }
    }
}

using Application.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Queries.Dtos;

namespace Application.Queries
{
    public interface IContactQueries
    {
        IEnumerable<ContactDto> GetContacts(Guid userId);
        Option<ContactDto> GetContact(Guid id, Guid userId);
    }
}

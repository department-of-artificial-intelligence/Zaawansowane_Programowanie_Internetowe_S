using Application.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Queries.Dtos
{
    public record ContactDto(
     Guid Id,
     string FirstName,
     string LastName,
     string Sex,
     int Age
 )
    {
        public static ContactDto FromDomain(Contact contact) =>
            new(
                contact.Id,
                contact.FirstName.Value, // jeśli FirstName to Value Object (np. .Value lub ToString())
                contact.LastName.Value,
                contact.Sex.ToString(),
                contact.Age.Value
            );
    }
}

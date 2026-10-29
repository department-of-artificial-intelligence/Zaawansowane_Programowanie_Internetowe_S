using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Queries.Dtos
{
    public record UserDto(
         Guid Id,
         string UserName,
         string? FirstName,
         string? LastName,
         DateOnly? DateOfBirth,
         string? Address,
         string Role
    );
}

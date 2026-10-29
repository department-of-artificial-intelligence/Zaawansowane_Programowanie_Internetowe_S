using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCases.Dtos
{
    public record UpdateUserProfileDto(
     Guid UserId,
     string? FirstName,
     string? LastName,
     DateOnly? DateOfBirth,
     string? Address
 );
}

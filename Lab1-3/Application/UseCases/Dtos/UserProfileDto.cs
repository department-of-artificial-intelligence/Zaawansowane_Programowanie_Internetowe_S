using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System;
using Application.Domain;
using Application.Utilities;

namespace Application.UseCases.Dtos;

public record UserProfileDto(string FirstName, string LastName, DateTime? BirthDate, string Address)
{
    public Result<UserProfile> ToDomain() =>
        UserProfile.Create(FirstName, LastName, BirthDate, Address);
}

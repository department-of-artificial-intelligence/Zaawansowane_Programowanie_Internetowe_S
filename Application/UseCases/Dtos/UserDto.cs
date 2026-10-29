using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System;
using Application.Domain;

namespace Application.UseCases.Dtos;

public record UserDto(Guid Id, string UserName, string Role, UserProfileDto? Profile)
{
    public static UserDto FromDomain(User user) => new(
        user.Id,
        user.UserName,
        user.Role.ToString(),
        user.Profile is not null
            ? new UserProfileDto(user.Profile.FirstName, user.Profile.LastName, user.Profile.BirthDate, user.Profile.Address)
            : null
    );
}

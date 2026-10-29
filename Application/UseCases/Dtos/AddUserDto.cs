using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Domain;
using Application.Utilities;

namespace Application.UseCases.Dtos;

public record AddUserDto(
    string UserName,
    string Password,
    UserRole Role = UserRole.User,
    UserProfileDto? Profile = null
)
{
    public Result<User> ToDomain() =>
        Profile is null
            ? User.Create(UserName, Password, Role)
            : Profile.ToDomain().Bind(p => User.Create(UserName, Password, Role, p));
}

using Application.Domain;
using Application.UseCases.Dtos;
using Application.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UseCases
{
    public interface IUserUseCases
    {
        Result<AddUserResult> AddUser(AddUserDto dto);
        Result<LoginUserResult> Login(LoginUserDto dto);
        Result<Unit> RemoveUser(Guid id);
        IEnumerable<UserDto> GetUsers();
        Result<Unit> UpdateProfile(Guid userId, UserProfileDto profileDto);
        Result<Unit> ChangeRole(Guid userId, UserRole newRole);
        Result<LoginUserResult> RefreshToken(RefreshTokenDto dto);
    }
}

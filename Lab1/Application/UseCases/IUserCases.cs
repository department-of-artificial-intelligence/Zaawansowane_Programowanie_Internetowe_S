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
        Result<Unit> UpdateProfile(UpdateUserProfileDto dto);
        Result<Unit> ChangeRole(ChangeUserRoleDto dto);
        Result<LoginUserResult> RefreshToken(RefreshTokenDto dto);
        Result<Unit> RemoveUser(Guid id);
    }
}

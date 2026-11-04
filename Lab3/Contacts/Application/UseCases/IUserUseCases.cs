using Application.UseCases.Dtos;
using Application.Utilities;

namespace Application.UseCases;

public interface IUserUseCases
{
    Result<AddUserResult> AddUser(AddUserDto dto);
    Result<LoginUserResult> Login(LoginUserDto dto);
    Result<LoginUserResult> Refresh(RefreshTokenDto dto);
    Result<Unit> RemoveUser(Guid id);
    Result<Unit> UpdateProfile(UpdateUserProfileDto dto);
    Result<Unit> ChangeRole(ChangeUserRoleDto dto);
}

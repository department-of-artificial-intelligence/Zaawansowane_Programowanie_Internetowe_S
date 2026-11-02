using Application.UseCases.Dtos;
using Application.Utilities;
namespace Application.UseCases;

public interface IUserUseCases
{
    Result<AddUserResult> AddUser(AddUserDto dto);
    Result<LoginUserResult> Login(LoginUserDto dto);
    Result<LoginUserResult> RefreshToken(RefreshTokenDto dto);
    Result<Unit> Logout(Guid userId);
    Result<Unit> UpdateProfile(UpdateProfileDto dto);
    Result<Unit> ChangePassword(ChangePasswordDto dto);
    Result<Unit> ChangeRole(ChangeRoleDto dto);
    Result<Unit> RemoveUser(RemoveUserDto dto);
    Result<Unit> EnsureAdminExists(string userName, string password);
}

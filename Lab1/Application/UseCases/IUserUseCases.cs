using Application.UseCases.Dtos;
using Application.Utilities;
using Application.Domain;

namespace Application.UseCases;

public interface IUserUseCases
{
    Result<AddUserResult> AddUser(AddUserDto dto);
    Result<LoginUserResult> Login(LoginUserDto dto);
    Result<Unit> RemoveUser(Guid id);
    Result<Unit> UpdateProfile(UpdateProfileDto dto);
	Result<Unit> ChangeRole(Guid userId, UserRole newRole);
	Result<LoginUserResult> RefreshToken(string refreshToken);
}
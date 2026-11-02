using Application.UseCases.Dtos;
using Application.Queries.Dtos;
using Application.Utilities;
namespace Application.UseCases;
public interface IUserUseCases
{
    Result<AddUserResult> AddUser(AddUserDto dto);
    Result<LoginUserResult> Login(LoginUserDto dto);
    Result<TokenResponseDto> RefreshToken(RefreshTokenDto dto);
    Result<IEnumerable<UserDto>> GetUsers();
    Result<Unit> UpdateUser(Guid userId, UpdateUserDto dto);
    Result<Unit> RemoveUser(Guid userId);
    Result<Unit> ChangeRole(Guid userId, ChangeUserRoleDto dto);
}

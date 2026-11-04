using Application.UseCases.Dtos;
using Application.Utilities;
using Application.Domain;
namespace Application.UseCases;
public interface IUserUseCases
{
    Result<AddUserResult> AddUser(AddUserDto dto);
    Result<LoginUserResult> Login(LoginUserDto dto);
    IEnumerable<UserDto> GetAll();
    Result<Unit> UpdateProfile(Guid id, UpdateProfileDto dto);
    Result<Unit> ChangeRole(Guid id, UserRole role);
    Result<Unit> Remove(Guid id);
    Result<LoginUserResult> Refresh(RefreshTokenDto dto);
}

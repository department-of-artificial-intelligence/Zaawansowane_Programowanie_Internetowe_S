using Application.UseCases.Dtos;
using Application.Utilities;

namespace Application.UseCases;

public interface IUserUseCases
{
    Result<AddUserResult> AddUser(AddUserDto dto);
    Result<LoginUserResult> Login(LoginUserDto dto);

    // Zadanie 10 - wymiana refresh tokenu na nową parę tokenów oraz wylogowanie.
    Result<LoginUserResult> Refresh(string refreshToken);
    Result<Unit> Logout(Guid userId);

    // Zadanie 5 - zarządzanie użytkownikami.
    Result<Unit> UpdateProfile(UpdateProfileDto dto);
    Result<Unit> ChangePassword(ChangePasswordDto dto);
    Result<Unit> RemoveUser(Guid userId, Guid currentUserId);

    // Zadanie 6 - nadawanie ról.
    Result<Unit> ChangeRole(ChangeRoleDto dto);
    Result<Unit> EnsureAdminExists(AddUserDto dto);
}

using Application.Domain;
namespace Application.UseCases.Dtos;

// Zadanie 10
public record RefreshTokenDto(string RefreshToken);

// Zadanie 4/5 (UserId z tokenu - użytkownik edytuje własny profil)
public record UpdateProfileDto(
    Guid UserId, string? FirstName, string? LastName, DateOnly? BirthDate, string? Address);

// Zadanie 3
public record ChangePasswordDto(Guid UserId, string CurrentPassword, string NewPassword);

// Zadanie 6 (RequestedBy = administrator wykonujący operację)
public record ChangeRoleDto(Guid UserId, UserRole Role, Guid RequestedBy);

// Zadanie 5
public record RemoveUserDto(Guid UserId, Guid RequestedBy);

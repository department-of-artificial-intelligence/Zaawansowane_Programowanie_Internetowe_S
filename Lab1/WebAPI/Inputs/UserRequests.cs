using Application.Domain;
namespace WebAPI.Inputs;

public record UpdateProfileRequest(
    string? FirstName, string? LastName, DateOnly? BirthDate, string? Address);

public record ChangePasswordRequest(string CurrentPassword, string NewPassword);

public record ChangeRoleRequest(UserRole Role);

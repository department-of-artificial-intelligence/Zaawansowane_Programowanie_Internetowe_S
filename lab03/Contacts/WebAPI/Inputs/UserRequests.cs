using Application.Domain;

namespace WebAPI.Inputs;

public record RefreshTokenRequest(string RefreshToken);
public record UpdateProfileRequest(
    string? FirstName, string? LastName, DateOnly? DateOfBirth, string? Address);
public record ChangePasswordRequest(string CurrentPassword, string NewPassword);
public record ChangeRoleRequest(UserRole Role);

using Application.Domain;

namespace Application.UseCases.Dtos;

public class ChangeUserRoleDto(UserRole role)
{
    public UserRole Role { get; } = role;
}
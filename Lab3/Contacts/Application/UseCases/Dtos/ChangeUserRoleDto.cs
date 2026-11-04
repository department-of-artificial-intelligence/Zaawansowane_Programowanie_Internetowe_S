using Application.Domain;

namespace Application.UseCases.Dtos;

public class ChangeUserRoleDto(Guid id, UserRole role)
{
    public Guid Id { get; } = id;
    public UserRole Role { get; } = role;
}

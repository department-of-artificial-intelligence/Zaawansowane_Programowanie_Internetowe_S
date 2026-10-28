using Application.Domain;

namespace Application.UseCases.Dtos;

public class ChangeRoleDto(Guid userId, UserRole role, Guid currentUserId)
{
    public Guid UserId { get; } = userId;
    public UserRole Role { get; } = role;
    public Guid CurrentUserId { get; } = currentUserId;
}

namespace Application.UseCases.Dtos;

public class UpdateProfileDto(
    Guid userId, string? firstName, string? lastName, DateOnly? dateOfBirth, string? address)
{
    public Guid UserId { get; } = userId;
    public string? FirstName { get; } = firstName;
    public string? LastName { get; } = lastName;
    public DateOnly? DateOfBirth { get; } = dateOfBirth;
    public string? Address { get; } = address;
}

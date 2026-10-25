using Application.Domain;
namespace Application.UseCases.Dtos;

public class UpdateReaderDto(Guid id, string firstName, string lastName, string email)
{
    public Guid Id { get; } = id;
    public string FirstName { get; } = firstName;
    public string LastName { get; } = lastName;
    public string Email { get; } = email;
}
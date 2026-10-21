using Application.Domain;
using Application.Utilities;

namespace Application.UseCases.Dtos;

public class AddReaderDto(string firstName, string lastName, string email)
{
    public string FirstName { get; } = firstName;
    public string LastName { get; } = lastName;
    public string Email { get; } = email;

    public Result<Reader> ToDomain() =>
        EmailAddress.Create(Email)
            .Bind(address => Reader.Create(FirstName, LastName, address));
}

using Application.Domain;

namespace Application.Queries.Dtos;

public class ReaderDto
{
    public Guid Id { get; }
    public string FirstName { get; }
    public string LastName { get; }
    public string Email { get; }

    private ReaderDto(Guid id, string firstName, string lastName, string email)
    {
        Id = id;
        FirstName = firstName;
        LastName = lastName;
        Email = email;
    }

    public static ReaderDto FromDomain(Reader reader) =>
        new(
            reader.Id,
            reader.Name.FirstName,
            reader.Name.LastName,
            reader.Email.Value
        );
}
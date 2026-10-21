using Application.Domain;
using Application.Utilities;

namespace Application.UseCases.Dtos;

public class RegisterReaderDto(string firstName, string lastName)
{
    public string FirstName { get; } = firstName;
    public string LastName { get; } = lastName;

    public Result<Reader> ToDomain() => Reader.Create(FirstName, LastName);
}

public class ReaderResult(Guid id, string firstName, string lastName)
{
    public Guid Id { get; } = id;
    public string FirstName { get; } = firstName;
    public string LastName { get; } = lastName;

    internal static ReaderResult FromDomain(Reader reader) =>
        new(reader.Id, reader.FirstName, reader.LastName);
}

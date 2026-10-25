using Application.Domain;

namespace Application.UseCases.Dtos;

public class AddReaderResult(Guid id, FullName name, EmailAddress email)
{
    public Guid Id { get; } = id;
    public FullName Name = name;
    public EmailAddress Email = email;

    internal static AddReaderResult FromDomain(Reader reader) =>
        new (reader.Id, reader.Name, reader.Email);
}
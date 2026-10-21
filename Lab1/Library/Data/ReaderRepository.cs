using Application.Domain;
using Application.Repositories;
using Application.Utilities;

namespace Data;

public class ReaderRepository(AppDbContext context) : IReaderRepository
{
    public void Add(Reader reader)
    {
        context.Readers.Add(reader);
    }

    public IEnumerable<Reader> GetAll() => context.Readers;

    public Option<Reader> GetById(Guid readerId)
    {
        var reader = context.Readers.Find(readerId);
        return reader is not null
            ? Option.Some(reader)
            : Option.None<Reader>();
    }

    public Option<Reader> GetByEmail(EmailAddress email)
    {
        var reader = context.Readers.SingleOrDefault(r => r.Email == email);
        return reader is not null
            ? Option.Some(reader)
            : Option.None<Reader>();
    }

    public void Remove(Reader reader)
    {
        context.Remove(reader);
    }
}

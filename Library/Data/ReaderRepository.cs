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

    public Option<Reader> GetById(Guid bookId)
    {
        var reader = context.Readers.Find(bookId);
        return reader is not null
            ? Option.Some(reader)
            : Option.None<Reader>();
    }
    public void Update (Reader reader)
    {
        context.Readers.Update(reader);
    }
    public void Remove(Reader reader)
    {
        context.Remove(reader);
    }
}
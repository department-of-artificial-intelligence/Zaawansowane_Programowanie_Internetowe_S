using Library.Application.Domain;
using Library.Application.Repositories;
using Library.Application.Utilities;

namespace Library.Data;

public class ReaderRepository(LibraryDbContext context) : IReaderRepository
{
    public void Add(Reader reader) => context.Readers.Add(reader);
    public IEnumerable<Reader> GetAll() => context.Readers.ToList();
    public Option<Reader> GetById(Guid id)
    {
        var reader = context.Readers.Find(id);
        return reader is not null ? Option.Some(reader) : Option.None<Reader>();
    }
}
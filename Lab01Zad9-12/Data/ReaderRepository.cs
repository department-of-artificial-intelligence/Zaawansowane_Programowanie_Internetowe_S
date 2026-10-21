using Application.Domain;
using Application.Services;
using Application.Utilities;

namespace Data;


public class ReaderRepository(AppDbContext context) : IReaderRepository
{
    public Result<Reader> GetById(Guid id)
    {
        var reader = context.Set<Reader>().Find(id);
        return reader is null ? Result.Failure<Reader>("Nie znaleziono czytelnika") : Result.Success(reader);
    }
    public void Add(Reader reader) => context.Set<Reader>().Add(reader);
}
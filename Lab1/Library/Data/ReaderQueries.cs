using Application.Queries;
using Application.Queries.Dtos;

namespace Data;

public class ReaderQueries(AppDbContext context) : IReaderQueries
{
    public IEnumerable<ReaderDto> GetReaders()
        => context.Readers.ToList().Select(ReaderDto.FromDomain);
}

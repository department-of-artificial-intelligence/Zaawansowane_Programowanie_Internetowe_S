using Application.Queries.Dtos;

namespace Application.Queries;

public interface IReaderQueries
{
    IEnumerable<ReaderDto> GetReaders();
}

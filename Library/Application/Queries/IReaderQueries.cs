using Application.Queries.Dtos;
using Application.Utilities;

namespace Application.UseCases;

public interface IReaderQueries
{
    IEnumerable<ReaderDto> GetAll();
    Option<ReaderDto> GetById(Guid id);
}
using Application.Queries.Dtos;
using Application.Utilities;

namespace Application.UseCases;

public interface IBookQueries
{
    IEnumerable<BookDto> GetAll();
    Option<BookDto> GetById(Guid id);
}
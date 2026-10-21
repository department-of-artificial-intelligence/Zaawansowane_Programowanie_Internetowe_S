using Application.Queries.Dtos;
using Application.Utilities;

namespace Application.Queries;

public interface IBookQueries
{
    IEnumerable<BookDto> GetBooks();
    Option<BookDto> GetBook(Guid id);
}

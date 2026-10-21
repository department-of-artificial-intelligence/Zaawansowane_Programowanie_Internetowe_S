using Application.Queries;
using Application.Queries.Dtos;
using Application.Utilities;

namespace Data;

public class BookQueries(AppDbContext context) : IBookQueries
{
    public IEnumerable<BookDto> GetBooks()
        => context.Books.ToList().Select(BookDto.FromDomain);

    public Option<BookDto> GetBook(Guid id)
    {
        var book = context.Books.Find(id);
        return book is not null
            ? Option.Some(BookDto.FromDomain(book))
            : Option.None<BookDto>();
    }
}

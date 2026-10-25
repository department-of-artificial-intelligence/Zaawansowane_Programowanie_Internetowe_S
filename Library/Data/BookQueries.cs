using Application.Queries;
using Application.Queries.Dtos;
using Application.Utilities;
using Application.Domain;
using Application.UseCases;

namespace Data;

public class BookQueries(AppDbContext context) : IBookQueries
{
    public IEnumerable<BookDto> GetAll()
        => context.Books.ToList().Select(BookDto.FromDomain);

    public Option<BookDto> GetById(Guid id)
    {
        var book = context.Books.Find(id);
        return book is not null
            ? Option.Some(BookDto.FromDomain(book))
            : Option.None<BookDto>();
    }
}
using Library.Application.Domain;
using Library.Application.Repositories;
using Library.Application.Utilities;

namespace Library.Data;

public class BookRepository(LibraryDbContext context) : IBookRepository
{
    public void Add(Book book) => context.Books.Add(book);
    public IEnumerable<Book> GetAll() => context.Books.ToList();
    public Option<Book> GetById(Guid id)
    {
        var book = context.Books.Find(id);
        return book is not null ? Option.Some(book) : Option.None<Book>();
    }
}
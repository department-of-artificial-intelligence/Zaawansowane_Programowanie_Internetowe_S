using Application.Domain;
using Application.Repositories;
using Application.Utilities;

namespace Data;

public class BookRepository(AppDbContext context) : IBookRepository
{
    public void Add(Book book)
    {
        context.Books.Add(book);
    }

    public IEnumerable<Book> GetAll() => context.Books;

    public Option<Book> GetById(Guid bookId)
    {
        var book = context.Books.Find(bookId);
        return book is not null
            ? Option.Some(book)
            : Option.None<Book>();
    }

    public Option<Book> GetByIsbn(Isbn isbn)
    {
        var book = context.Books.SingleOrDefault(b => b.Isbn == isbn);
        return book is not null
            ? Option.Some(book)
            : Option.None<Book>();
    }

    public void Remove(Book book)
    {
        context.Remove(book);
    }
}

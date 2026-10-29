using System;
using System.Linq;
using Application.Domain;
using Application.Repositories;
using Application.Utilities;

namespace Data;

public class BookRepository : IBookRepository
{
    private readonly AppDbContext _context;

    public BookRepository(AppDbContext context)
    {
        _context = context;
    }

    public void Add(Book book) => _context.Books.Add(book);

    public Option<Book> GetById(Guid id)
    {
        var book = _context.Books.Find(id);
        return book is not null
            ? Option.Some(book)
            : Option.None<Book>();
    }
}

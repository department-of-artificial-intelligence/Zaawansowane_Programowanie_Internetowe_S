using Application.Domain;
using Application.Services;
using Application.Utilities;
using Application.Repositories;

namespace Data;

public class BookRepository(AppDbContext context) : IBookRepository
{
    public Result<Book> GetById(Guid id)
    {
        var book = context.Set<Book>().Find(id);
        return book is null ? Result.Failure<Book>("Nie znaleziono książki") : Result.Success(book);
    }
    public void Add(Book book) => context.Set<Book>().Add(book);
}
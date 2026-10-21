using Application.Domain;
using Application.Utilities;

namespace Application.Repositories;

public interface IBookRepository
{
    IEnumerable<Book> GetAll();
    Option<Book> GetById(Guid bookId);
    Option<Book> GetByIsbn(Isbn isbn);
    void Add(Book book);
    void Remove(Book book);
}

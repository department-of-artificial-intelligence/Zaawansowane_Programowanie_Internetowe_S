using Application.Domain;
using Application.Utilities;

namespace Application.Repositories;

public interface IBookRepository
{
    IEnumerable<Book> GetAll();
    Option<Book> GetById(Guid bookId);
    void Add(Book book);
    void Update(Book book);
    void Remove(Book book);
}
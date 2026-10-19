using Application.Utilities;
using Library.Application.Domain;

namespace Library.Application.Repositories;

public interface IBookRepository
{
    Option<Book> GetById(Guid id);
    IEnumerable<Book> GetAll();
    void Add(Book book);
}
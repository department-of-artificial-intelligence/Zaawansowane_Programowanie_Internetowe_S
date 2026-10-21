using Application.Domain;
using Application.Utilities;

namespace Application.Repositories;

public interface IBookRepository
{
    Result<Book> GetById(Guid id);
    void Add(Book book);
}
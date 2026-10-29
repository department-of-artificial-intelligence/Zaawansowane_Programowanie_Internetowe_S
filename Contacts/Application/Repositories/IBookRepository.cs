// Application/Repositories/IBookRepository.cs
using Application.Domain;
using Application.Utilities;

namespace Application.Repositories;

public interface IBookRepository
{
    void Add(Book book);
    Option<Book> GetById(Guid id);
}
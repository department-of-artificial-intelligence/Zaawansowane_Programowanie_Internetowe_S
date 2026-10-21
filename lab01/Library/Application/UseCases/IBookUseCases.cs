using Application.UseCases.Dtos;
using Application.Utilities;

namespace Application.UseCases;

public interface IBookUseCases
{
    Result<BookResult> AddBook(AddBookDto addBookDto);
    Result<Unit> RemoveBook(Guid bookId);
}

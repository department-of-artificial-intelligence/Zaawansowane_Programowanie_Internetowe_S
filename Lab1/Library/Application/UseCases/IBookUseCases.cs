using Application.UseCases.Dtos;
using Application.Utilities;

namespace Application.UseCases;

public interface IBookUseCases
{
    Result<AddBookResult> AddBook(AddBookDto addBookDto);
    Result<Unit> UpdateBook(UpdateBookDto updateBookDto);
    Result<Unit> RemoveBook(Guid id);
}

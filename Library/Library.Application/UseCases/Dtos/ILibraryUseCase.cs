using Application.Utilities;
using Library.Application.UseCases.Dtos;

namespace Library.Application.UseCases;

public interface ILibraryUseCases
{
    Result<Guid> AddBook(AddBookDto dto);
    Result<Guid> RegisterReader(AddReaderDto dto);
    Result<Unit> BorrowBook(BorrowBookDto dto);
    Result<Unit> ReturnBook(ReturnBookDto dto);
}
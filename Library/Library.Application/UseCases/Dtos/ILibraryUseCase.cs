using Library.Application.UseCases.Dtos;
using Library.Application.Utilities;

namespace Library.Application.UseCases;

public interface ILibraryUseCases
{
    Result<Guid> AddBook(AddBookDto dto);
    Result<Guid> RegisterReader(AddReaderDto dto);
    Result<Unit> BorrowBook(BorrowBookDto dto);
    Result<Unit> ReturnBook(ReturnBookDto dto);
}
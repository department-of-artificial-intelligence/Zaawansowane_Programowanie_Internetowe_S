using Application.UseCases.Dtos;
using Application.Utilities;

namespace Application.UseCases;

public interface ILibraryUseCases
{
    Result<AddBookResult> AddBook(AddBookDto dto);
    Result<RegisterReaderResult> RegisterReader(RegisterReaderDto dto);
    Result<Unit> BorrowBook(BorrowBookDto dto);
    Result<Unit> ReturnBook(ReturnBookDto dto);
}
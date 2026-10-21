using Application.Domain;
using Application.UseCases.Dtos;
using Application.Utilities;
using Application.OutputPorts;
using Application.Repositories;

namespace Application.Services;

public class LibraryService(
    IBookRepository bookRepository,
    IReaderRepository readerRepository,
    IUnitOfWork unitOfWork)
{
    public Result<Unit> BorrowBook(BorrowBookDto dto)
    {
        return readerRepository.GetById(dto.ReaderId)
            .Bind(reader => bookRepository.GetById(dto.BookId)
                .Bind(book => book.Borrow()
                    .Bind(_ => reader.BorrowBook(book.Id))
                )
            )
            .Tap(_ => unitOfWork.Save());
    }
}
using Library.Application.Domain;
using Library.Application.OutputPorts;
using Library.Application.Repositories;
using Library.Application.UseCases;
using Library.Application.UseCases.Dtos;
using Library.Application.Utilities;

namespace Library.Application.Services;

public class LibraryServices(
    IBookRepository bookRepo,
    IReaderRepository readerRepo,
    ILoanRepository loanRepo,
    IUnitOfWork uow
) : ILibraryUseCases
{
    public Result<Guid> AddBook(AddBookDto dto) =>
        Isbn.Create(dto.Isbn)
            .Bind(isbn => Book.Create(dto.Title, dto.Author, isbn))
            .Tap(bookRepo.Add)
            .Tap(_ => uow.Save())
            .Map(b => b.Id);

    public Result<Guid> RegisterReader(AddReaderDto dto) =>
        Reader.Create(dto.FullName, dto.Email)
            .Tap(readerRepo.Add)
            .Tap(_ => uow.Save())
            .Map(r => r.Id);

    public Result<Unit> BorrowBook(BorrowBookDto dto) =>
        bookRepo.GetById(dto.BookId)
            .Required($"Nie znaleziono książki o ID {dto.BookId}")
            .Bind(book => readerRepo.GetById(dto.ReaderId)
                .Required($"Nie znaleziono czytelnika o ID {dto.ReaderId}")
                .Bind(reader => reader.CanBorrow()
                    .Bind(_ => book.MarkAsBorrowed())
                    .Bind(_ => Loan.Create(book.Id, reader.Id))
                    .Tap(loanRepo.Add)
                    .Tap(_ => reader.IncrementLoan())
                    .Map(_ => Unit.Instance)))
            .Tap(_ => uow.Save());

    public Result<Unit> ReturnBook(ReturnBookDto dto) =>
        bookRepo.GetById(dto.BookId)
            .Required($"Nie znaleziono książki o ID {dto.BookId}")
            .Bind(book => loanRepo.GetActiveLoan(dto.BookId)
                .Required("Książka nie posiada aktywnego wypożyczenia.")
                .Bind(loan => readerRepo.GetById(loan.ReaderId)
                    .Required("Nie znaleziono czytelnika powiązanego z wypożyczeniem.")
                    .Bind(reader => loan.CompleteReturn()
                        .Bind(_ => book.MarkAsReturned())
                        .Tap(_ => reader.DecrementLoan()))))
            .Tap(_ => uow.Save());
}
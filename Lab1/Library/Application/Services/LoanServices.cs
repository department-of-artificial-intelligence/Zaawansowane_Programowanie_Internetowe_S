using Application.Domain;
using Application.DomainServices;
using Application.OutputPorts;
using Application.Repositories;
using Application.UseCases;
using Application.UseCases.Dtos;
using Application.Utilities;

namespace Application.Services;

public class LoanServices(
    ILoanRepository loanRepository,
    IBookRepository bookRepository,
    IReaderRepository readerRepository,
    IReservationRepository reservationRepository,
    IUnitOfWork unitOfWork,
    LibraryRules libraryRules
) : ILoanUseCases
{
    public Result<BorrowBookResult> BorrowBook(BorrowBookDto borrowBookDto)
        => readerRepository.GetById(borrowBookDto.ReaderId)
            .Required($"Brak czytelnika (id: {borrowBookDto.ReaderId})")
            .Bind(reader =>
                bookRepository
                    .GetById(borrowBookDto.BookId)
                    .Required($"Brak książki (id: {borrowBookDto.BookId})")
                    .Map(book => (reader, book)))
            .Bind(param =>
                libraryRules
                    .EnsureCanBorrow(param.reader, param.book)
                    .Map(_ => param))
            .Bind(param => param.book.TakeCopy().Map(_ => param))
            .Tap(param => RemoveReservation(param.reader.Id, param.book.Id))
            .Bind(param => Loan.Create(
                param.book.Id,
                param.reader.Id,
                DateOnly.FromDateTime(DateTime.Today)))
            .Tap(loanRepository.Add)
            .Tap(_ => unitOfWork.Save())
            .Map(BorrowBookResult.FromDomain);

    public Result<Unit> ReturnBook(Guid loanId)
        => loanRepository.GetById(loanId)
            .Required($"Brak wypożyczenia (id: {loanId})")
            .Bind(loan =>
                loan.Return(DateOnly.FromDateTime(DateTime.Today))
                    .Map(_ => loan))
            .Bind(loan =>
                bookRepository
                    .GetById(loan.BookId)
                    .Required($"Brak książki (id: {loan.BookId})"))
            .Bind(book => book.ReturnCopy())
            .Tap(_ => unitOfWork.Save());

    private void RemoveReservation(Guid readerId, Guid bookId) =>
        reservationRepository
            .GetByBook(bookId)
            .Where(reservation => reservation.ReaderId == readerId)
            .ToList()
            .ForEach(reservationRepository.Remove);
}

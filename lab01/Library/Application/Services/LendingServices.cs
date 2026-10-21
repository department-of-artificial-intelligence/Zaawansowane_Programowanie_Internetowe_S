using Application.Domain;
using Application.DomainServices;
using Application.OutputPorts;
using Application.Repositories;
using Application.UseCases;
using Application.UseCases.Dtos;
using Application.Utilities;

namespace Application.Services;

public class LendingServices(
    IBookRepository bookRepository,
    IReaderRepository readerRepository,
    ILoanRepository loanRepository,
    IReservationRepository reservationRepository,
    LendingPolicy lendingPolicy,
    IClock clock,
    IUnitOfWork unitOfWork
) : ILendingUseCases
{
    public Result<LoanResult> BorrowBook(BorrowBookDto borrowBookDto)
        => FindBookAndReader(borrowBookDto.BookId, borrowBookDto.ReaderId)
            .Bind(param => lendingPolicy.Borrow(param.book, param.reader, clock.Today))
            .Tap(loanRepository.Add)
            .Tap(loan => FulfillReservation(loan.BookId, loan.ReaderId))
            .Tap(_ => unitOfWork.Save())
            .Map(LoanResult.FromDomain);

    public Result<Unit> ReturnBook(Guid loanId)
        => FindLoan(loanId)
            .Bind(loan => loan.Return(clock.Today))
            .Tap(_ => unitOfWork.Save());

    public Result<LoanResult> ExtendLoan(Guid loanId)
        => FindLoan(loanId)
            .Bind(loan => lendingPolicy.Extend(loan, clock.Today).Map(_ => loan))
            .Tap(_ => unitOfWork.Save())
            .Map(LoanResult.FromDomain);

    public Result<ReservationResult> ReserveBook(ReserveBookDto reserveBookDto)
        => FindBookAndReader(reserveBookDto.BookId, reserveBookDto.ReaderId)
            .Bind(param => lendingPolicy.Reserve(param.book, param.reader, clock.Now))
            .Tap(reservationRepository.Add)
            .Tap(_ => unitOfWork.Save())
            .Map(ReservationResult.FromDomain);

    public Result<Unit> CancelReservation(Guid reservationId)
        => reservationRepository
            .GetById(reservationId)
            .Required($"Brak rezerwacji (id: {reservationId})")
            .Bind(reservation => reservation.Cancel())
            .Tap(_ => unitOfWork.Save());

    private void FulfillReservation(Guid bookId, Guid readerId)
    {
        foreach (var reservation in reservationRepository
                     .GetActiveByReader(readerId)
                     .Where(r => r.BookId == bookId))
        {
            reservation.Fulfill();
        }
    }

    private Result<(Book book, Reader reader)> FindBookAndReader(Guid bookId, Guid readerId)
        => bookRepository
            .GetById(bookId)
            .Required($"Brak książki (id: {bookId})")
            .Bind(book => readerRepository
                .GetById(readerId)
                .Required($"Brak czytelnika (id: {readerId})")
                .Map(reader => (book, reader)));

    private Result<Loan> FindLoan(Guid loanId)
        => loanRepository
            .GetById(loanId)
            .Required($"Brak wypożyczenia (id: {loanId})");
}

using Application.UseCases.Dtos;
using Application.Utilities;

namespace Application.UseCases;

public interface ILendingUseCases
{
    Result<LoanResult> BorrowBook(BorrowBookDto borrowBookDto);
    Result<Unit> ReturnBook(Guid loanId);
    Result<LoanResult> ExtendLoan(Guid loanId);
    Result<ReservationResult> ReserveBook(ReserveBookDto reserveBookDto);
    Result<Unit> CancelReservation(Guid reservationId);
}

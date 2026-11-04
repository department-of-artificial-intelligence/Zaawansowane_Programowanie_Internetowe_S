using Library.Domain;

namespace Library.UseCases;

public record AddBookDto(string Isbn, string Title, string Author);
public record AddReaderDto(string FirstName, string LastName, string Email);
public record BorrowBookDto(Guid BookId, Guid ReaderId, DateOnly BorrowedOn, DateOnly DueOn);
public record ReturnBookDto(Guid LoanId, DateOnly ReturnedOn);
public record ReserveBookDto(Guid BookId, Guid ReaderId, DateOnly ReservedOn);
public record CancelReservationDto(Guid ReservationId);
public record BookResult(Guid Id, string Isbn, string Title, string Author);
public record ReaderResult(Guid Id, string FirstName, string LastName, string Email);
public record LoanResult(Guid Id, Guid BookId, Guid ReaderId, DateOnly DueOn);
public record ReservationResult(Guid Id, Guid BookId, Guid ReaderId, DateOnly ReservedOn);

public interface ILibraryUseCases
{
    Application.Utilities.Result<BookResult> AddBook(AddBookDto dto);
    Application.Utilities.Result<ReaderResult> AddReader(AddReaderDto dto);
    Application.Utilities.Result<LoanResult> Borrow(BorrowBookDto dto);
    Application.Utilities.Result<Application.Utilities.Unit> Return(ReturnBookDto dto);
    Application.Utilities.Result<ReservationResult> Reserve(ReserveBookDto dto);
    Application.Utilities.Result<Application.Utilities.Unit> CancelReservation(
        CancelReservationDto dto);
}

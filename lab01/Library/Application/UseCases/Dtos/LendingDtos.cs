using Application.Domain;

namespace Application.UseCases.Dtos;

public class BorrowBookDto(Guid readerId, Guid bookId)
{
    public Guid ReaderId { get; } = readerId;
    public Guid BookId { get; } = bookId;
}

public class ReserveBookDto(Guid readerId, Guid bookId)
{
    public Guid ReaderId { get; } = readerId;
    public Guid BookId { get; } = bookId;
}

public class LoanResult(Guid id, Guid bookId, Guid readerId, DateOnly loanDate, DateOnly dueDate)
{
    public Guid Id { get; } = id;
    public Guid BookId { get; } = bookId;
    public Guid ReaderId { get; } = readerId;
    public DateOnly LoanDate { get; } = loanDate;
    public DateOnly DueDate { get; } = dueDate;

    internal static LoanResult FromDomain(Loan loan) =>
        new(loan.Id, loan.BookId, loan.ReaderId, loan.LoanDate, loan.DueDate);
}

public class ReservationResult(Guid id, Guid bookId, Guid readerId, DateTime createdAt)
{
    public Guid Id { get; } = id;
    public Guid BookId { get; } = bookId;
    public Guid ReaderId { get; } = readerId;
    public DateTime CreatedAt { get; } = createdAt;

    internal static ReservationResult FromDomain(Reservation reservation) =>
        new(reservation.Id, reservation.BookId, reservation.ReaderId, reservation.CreatedAt);
}

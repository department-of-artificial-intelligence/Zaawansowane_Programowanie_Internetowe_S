using Application.Utilities;

namespace Application.Domain;

public class Loan : Entity
{
    public const int LoanDays = 30;

    public Guid BookId { get; private set; }
    public Guid ReaderId { get; private set; }
    public DateOnly LoanDate { get; private set; }
    public DateOnly DueDate { get; private set; }
    public DateOnly? ReturnDate { get; private set; }

    public bool IsActive => ReturnDate is null;

    private Loan(
        Guid id,
        Guid bookId,
        Guid readerId,
        DateOnly loanDate) : base(id)
    {
        BookId = bookId;
        ReaderId = readerId;
        LoanDate = loanDate;
        DueDate = loanDate.AddDays(LoanDays);
    }

    public Result<Unit> Return(DateOnly returnDate)
    {
        if (!IsActive)
        {
            return Result.Failure<Unit>("Książka została już zwrócona.");
        }

        ReturnDate = returnDate;
        return Result.Success(Unit.Instance);
    }

    public static Result<Loan> Create(
        Guid bookId,
        Guid readerId,
        DateOnly loanDate) =>
        Result.Success(new Loan(Guid.NewGuid(), bookId, readerId, loanDate));
}

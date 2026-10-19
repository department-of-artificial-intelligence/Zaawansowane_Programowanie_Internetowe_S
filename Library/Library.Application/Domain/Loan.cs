using Application.Utilities;

namespace Library.Application.Domain;

public class Loan : Entity
{
    public Guid BookId { get; private set; }
    public Guid ReaderId { get; private set; }
    public DateTime LoanDate { get; private set; }
    public DateTime? ReturnDate { get; private set; }

    private Loan() : base(Guid.Empty) { }

    private Loan(Guid id, Guid bookId, Guid readerId, DateTime loanDate) : base(id)
    {
        BookId = bookId;
        ReaderId = readerId;
        LoanDate = loanDate;
    }

    public static Result<Loan> Create(Guid bookId, Guid readerId) =>
        Result.Success(new Loan(Guid.NewGuid(), bookId, readerId, DateTime.UtcNow));

    public Result<Unit> CompleteReturn()
    {
        if (ReturnDate.HasValue)
            return Result.Failure<Unit>("Wypożyczenie zostało już wcześniej zakończone.");

        ReturnDate = DateTime.UtcNow;
        return Result.Success(Unit.Instance);
    }
}
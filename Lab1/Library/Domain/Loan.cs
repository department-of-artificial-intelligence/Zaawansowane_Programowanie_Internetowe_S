using Application.Utilities;

namespace Library.Domain;

public sealed class Loan
{
    private Loan(Guid id, Guid bookId, Guid readerId, DateOnly borrowedOn, DateOnly dueOn)
    {
        Id = id;
        BookId = bookId;
        ReaderId = readerId;
        BorrowedOn = borrowedOn;
        DueOn = dueOn;
    }

    public Guid Id { get; }
    public Guid BookId { get; }
    public Guid ReaderId { get; }
    public DateOnly BorrowedOn { get; }
    public DateOnly DueOn { get; }
    public DateOnly? ReturnedOn { get; private set; }
    public bool IsActive => ReturnedOn is null;

    public static Result<Loan> Create(
        Guid bookId, Guid readerId, DateOnly borrowedOn, DateOnly dueOn) =>
        dueOn > borrowedOn
            ? Result.Success(new Loan(
                Guid.NewGuid(), bookId, readerId, borrowedOn, dueOn))
            : Result.Failure<Loan>("Termin zwrotu musi być późniejszy niż wypożyczenie.");

    public Result<Unit> Return(DateOnly date) =>
        !IsActive
            ? Result.Failure<Unit>("Wypożyczenie zostało już zwrócone.")
            : Result.Success(SetReturned(date));

    private Unit SetReturned(DateOnly date)
    {
        ReturnedOn = date;
        return Unit.Instance;
    }
}

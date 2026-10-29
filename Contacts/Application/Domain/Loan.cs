using System;
using Application.Utilities;

namespace Application.Domain;

public class Loan : Entity
{
    public Guid BookId { get; private set; }
    public Guid ReaderId { get; private set; }
    public DateTime LoanDate { get; private set; }
    public DateTime DueDate { get; private set; }
    public DateTime? ReturnDate { get; private set; }

    private Loan() : base(Guid.Empty) { }

    private Loan(Guid bookId, Guid readerId, int days) : base(Guid.NewGuid())
    {
        BookId = bookId;
        ReaderId = readerId;
        LoanDate = DateTime.Now;
        DueDate = DateTime.Now.AddDays(days);
    }

    public static Result<Loan> Create(Guid bookId, Guid readerId, int days = 14)
    {
        return Result.Success(new Loan(bookId, readerId, days));
    }

    public Result<Unit> CompleteReturn()
    {
        if (ReturnDate.HasValue)
            return Result.Failure<Unit>("Książka została już zwrócona.");

        ReturnDate = DateTime.Now;
        return Result.Success(Unit.Instance);
    }
}
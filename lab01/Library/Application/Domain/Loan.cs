using Application.Utilities;

namespace Application.Domain;

public class Loan : Entity
{
    public Guid BookId { get; private set; }
    public Guid ReaderId { get; private set; }
    public DateOnly LoanDate { get; private set; }
    public DateOnly DueDate { get; private set; }
    public DateOnly? ReturnDate { get; private set; }
    public bool Extended { get; private set; }

    public bool IsActive => ReturnDate is null;

    private Loan(Guid id, Guid bookId, Guid readerId, DateOnly loanDate, DateOnly dueDate)
        : base(id)
    {
        BookId = bookId;
        ReaderId = readerId;
        LoanDate = loanDate;
        DueDate = dueDate;
    }

    public bool IsOverdue(DateOnly today) => IsActive && today > DueDate;

    // Wypożyczenie tworzy wyłącznie LendingPolicy, która sprawdza reguły biznesowe.
    internal static Loan Create(Guid bookId, Guid readerId, DateOnly today) =>
        new(Guid.NewGuid(), bookId, readerId, today, today.AddDays(LibraryRules.LoanPeriodDays));

    public Result<Unit> Return(DateOnly today)
    {
        if (!IsActive)
        {
            return Result.Failure<Unit>("Książka została już zwrócona.");
        }

        ReturnDate = today;
        return Result.Success(Unit.Instance);
    }

    internal Result<Unit> Extend(DateOnly today)
    {
        if (!IsActive)
            return Result.Failure<Unit>("Nie można przedłużyć zakończonego wypożyczenia.");
        if (Extended)
            return Result.Failure<Unit>("Wypożyczenie można przedłużyć tylko raz.");
        if (IsOverdue(today))
            return Result.Failure<Unit>("Nie można przedłużyć przeterminowanego wypożyczenia.");

        DueDate = DueDate.AddDays(LibraryRules.ExtensionDays);
        Extended = true;
        return Result.Success(Unit.Instance);
    }
}

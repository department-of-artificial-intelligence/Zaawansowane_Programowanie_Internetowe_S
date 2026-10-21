using Application.Domain;
using Application.Repositories;
using Application.Utilities;

namespace Application.DomainServices;

// Reguły biznesowe obejmujące kilka agregatów (książka, czytelnik, wypożyczenia, rezerwacje).
public class LendingPolicy(
    ILoanRepository loanRepository,
    IReservationRepository reservationRepository)
{
    public Result<Loan> Borrow(Book book, Reader reader, DateOnly today)
    {
        var readerLoans = loanRepository.GetActiveByReader(reader.Id).ToList();

        return Result.Combine(
                Check(readerLoans.Count < LibraryRules.MaxActiveLoans,
                    $"Czytelnik może mieć maksymalnie {LibraryRules.MaxActiveLoans} wypożyczonych książek."),
                Check(!readerLoans.Any(l => l.IsOverdue(today)),
                    "Czytelnik ma przeterminowane wypożyczenia."),
                Check(readerLoans.All(l => l.BookId != book.Id),
                    "Czytelnik ma już wypożyczony egzemplarz tej książki."))
            .Bind(_ => EnsureCopyAvailableFor(book, reader.Id))
            .Map(_ => Loan.Create(book.Id, reader.Id, today));
    }

    public Result<Reservation> Reserve(Book book, Reader reader, DateTime now)
    {
        var readerReservations = reservationRepository.GetActiveByReader(reader.Id).ToList();

        return Result.Combine(
                Check(EnsureCopyAvailableFor(book, reader.Id).Match(_ => false, _ => true),
                    "Książka jest dostępna - można ją od razu wypożyczyć."),
                Check(readerReservations.All(r => r.BookId != book.Id),
                    "Czytelnik ma już rezerwację tej książki."),
                Check(readerReservations.Count < LibraryRules.MaxActiveReservations,
                    $"Czytelnik może mieć maksymalnie {LibraryRules.MaxActiveReservations} rezerwacji."),
                Check(loanRepository.GetActiveByReader(reader.Id).All(l => l.BookId != book.Id),
                    "Czytelnik ma już wypożyczony egzemplarz tej książki."))
            .Map(_ => Reservation.Create(book.Id, reader.Id, now));
    }

    public Result<Unit> Extend(Loan loan, DateOnly today) =>
        Check(!reservationRepository.GetActiveByBook(loan.BookId).Any(),
                "Nie można przedłużyć wypożyczenia - książka jest zarezerwowana.")
            .Bind(_ => loan.Extend(today));

    public Result<Unit> EnsureBookCanBeRemoved(Book book) =>
        Check(!loanRepository.GetActiveByBook(book.Id).Any(),
            "Nie można usunąć książki, której egzemplarze są wypożyczone.");

    public Result<Unit> EnsureReaderCanBeRemoved(Reader reader) =>
        Check(!loanRepository.GetActiveByReader(reader.Id).Any(),
            "Nie można usunąć czytelnika, który nie zwrócił wszystkich książek.");

    // Wolny egzemplarz należy się czytelnikom według kolejności rezerwacji.
    private Result<Unit> EnsureCopyAvailableFor(Book book, Guid readerId)
    {
        var availableCopies = book.Copies - loanRepository.GetActiveByBook(book.Id).Count();
        var queue = reservationRepository
            .GetActiveByBook(book.Id)
            .OrderBy(r => r.CreatedAt)
            .ToList();
        var readerPosition = queue.FindIndex(r => r.ReaderId == readerId);
        var reservationsAhead = readerPosition >= 0 ? readerPosition : queue.Count;

        if (availableCopies <= 0)
            return Result.Failure<Unit>("Brak wolnych egzemplarzy książki.");
        if (availableCopies <= reservationsAhead)
            return Result.Failure<Unit>("Wolne egzemplarze są zarezerwowane przez innych czytelników.");
        return Result.Success(Unit.Instance);
    }

    private static Result<Unit> Check(bool condition, string errorMessage) =>
        condition
            ? Result.Success(Unit.Instance)
            : Result.Failure<Unit>(errorMessage);
}

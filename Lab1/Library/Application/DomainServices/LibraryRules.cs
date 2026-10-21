using Application.Domain;
using Application.Repositories;
using Application.Utilities;

namespace Application.DomainServices;

public class LibraryRules(
    ILoanRepository loanRepository,
    IReservationRepository reservationRepository)
{
    public const int MaxActiveLoans = 3;

    public Result<Unit> EnsureCanBorrow(Reader reader, Book book)
    {
        var activeLoans = loanRepository
            .GetByReader(reader.Id)
            .Where(loan => loan.IsActive)
            .ToList();

        var reservations = reservationRepository
            .GetByBook(book.Id)
            .ToList();

        var hasReservation = reservations.Any(
            reservation => reservation.ReaderId == reader.Id);
        var reservedForOthers = hasReservation ? 0 : reservations.Count;

        return Result.Combine(
            Check(
                activeLoans.Count < MaxActiveLoans,
                $"Czytelnik osiągnął limit wypożyczeń ({MaxActiveLoans})."),
            Check(
                !activeLoans.Any(loan => loan.BookId == book.Id),
                "Czytelnik ma już wypożyczoną tę książkę."),
            Check(
                book.AvailableCopies > reservedForOthers,
                "Brak wolnego egzemplarza dla tego czytelnika."));
    }

    public Result<Unit> EnsureCanReserve(Reader reader, Book book)
    {
        var hasLoan = loanRepository
            .GetByReader(reader.Id)
            .Any(loan => loan.IsActive && loan.BookId == book.Id);

        var reservations = reservationRepository
            .GetByBook(book.Id)
            .ToList();

        return Result.Combine(
            Check(
                !hasLoan,
                "Czytelnik ma już wypożyczoną tę książkę."),
            Check(
                !reservations.Any(reservation => reservation.ReaderId == reader.Id),
                "Czytelnik już zarezerwował tę książkę."),
            Check(
                reservations.Count >= book.AvailableCopies,
                "Książka jest dostępna, można ją wypożyczyć bez rezerwacji."));
    }

    public Result<Unit> EnsureBookCanBeRemoved(Book book) =>
        Result.Combine(
            Check(
                !loanRepository.GetByBook(book.Id).Any(),
                "Książka ma historię wypożyczeń."),
            Check(
                !reservationRepository.GetByBook(book.Id).Any(),
                "Książka ma rezerwacje."));

    public Result<Unit> EnsureReaderCanBeRemoved(Reader reader) =>
        Result.Combine(
            Check(
                !loanRepository.GetByReader(reader.Id).Any(),
                "Czytelnik ma historię wypożyczeń."),
            Check(
                !reservationRepository.GetByReader(reader.Id).Any(),
                "Czytelnik ma rezerwacje."));

    private static Result<Unit> Check(bool condition, string errorMessage) =>
        condition
            ? Result.Success(Unit.Instance)
            : Result.Failure<Unit>(errorMessage);
}

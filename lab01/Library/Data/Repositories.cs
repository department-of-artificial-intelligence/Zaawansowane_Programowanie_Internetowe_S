using Application.Domain;
using Application.Repositories;
using Application.Utilities;

namespace Data;

public class BookRepository(AppDbContext context) : IBookRepository
{
    public Option<Book> GetById(Guid bookId) => ToOption(context.Books.Find(bookId));

    public Option<Book> GetByIsbn(Isbn isbn) =>
        ToOption(context.Books.SingleOrDefault(book => book.Isbn == isbn));

    public void Add(Book book) => context.Books.Add(book);

    public void Remove(Book book) => context.Remove(book);

    private static Option<T> ToOption<T>(T? value) =>
        value is not null ? Option.Some(value) : Option.None<T>();
}

public class ReaderRepository(AppDbContext context) : IReaderRepository
{
    public Option<Reader> GetById(Guid readerId)
    {
        var reader = context.Readers.Find(readerId);
        return reader is not null
            ? Option.Some(reader)
            : Option.None<Reader>();
    }

    public void Add(Reader reader) => context.Readers.Add(reader);

    public void Remove(Reader reader) => context.Remove(reader);
}

public class LoanRepository(AppDbContext context) : ILoanRepository
{
    public Option<Loan> GetById(Guid loanId)
    {
        var loan = context.Loans.Find(loanId);
        return loan is not null
            ? Option.Some(loan)
            : Option.None<Loan>();
    }

    public IEnumerable<Loan> GetActiveByReader(Guid readerId) =>
        context.Loans.Where(l => l.ReaderId == readerId && l.ReturnDate == null).ToList();

    public IEnumerable<Loan> GetActiveByBook(Guid bookId) =>
        context.Loans.Where(l => l.BookId == bookId && l.ReturnDate == null).ToList();

    public void Add(Loan loan) => context.Loans.Add(loan);
}

public class ReservationRepository(AppDbContext context) : IReservationRepository
{
    public Option<Reservation> GetById(Guid reservationId)
    {
        var reservation = context.Reservations.Find(reservationId);
        return reservation is not null
            ? Option.Some(reservation)
            : Option.None<Reservation>();
    }

    public IEnumerable<Reservation> GetActiveByReader(Guid readerId) =>
        context.Reservations
            .Where(r => r.ReaderId == readerId && r.Status == ReservationStatus.Active)
            .ToList();

    public IEnumerable<Reservation> GetActiveByBook(Guid bookId) =>
        context.Reservations
            .Where(r => r.BookId == bookId && r.Status == ReservationStatus.Active)
            .ToList();

    public void Add(Reservation reservation) => context.Reservations.Add(reservation);
}

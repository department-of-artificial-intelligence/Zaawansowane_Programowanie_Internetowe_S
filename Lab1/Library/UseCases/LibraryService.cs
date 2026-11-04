using Application.Utilities;
using Library.Domain;
using Library.Ports;

namespace Library.UseCases;

public sealed class LibraryService(
    IBookRepository books,
    IReaderRepository readers,
    ILoanRepository loans,
    IReservationRepository reservations,
    IUnitOfWork unitOfWork) : ILibraryUseCases
{
    public Result<BookResult> AddBook(AddBookDto dto) =>
        Isbn.Create(dto.Isbn)
            .Bind(isbn => NotEmpty(dto.Title, "Tytuł jest wymagany.")
                .Bind(title => NotEmpty(dto.Author, "Autor jest wymagany.")
                    .Map(author => new Book(Guid.NewGuid(), isbn, title, author))))
            .Tap(books.Add)
            .Tap(_ => unitOfWork.Save())
            .Map(book => new BookResult(
                book.Id, book.Isbn.Value, book.Title, book.Author));

    public Result<ReaderResult> AddReader(AddReaderDto dto) =>
        NotEmpty(dto.FirstName, "Imię jest wymagane.")
            .Bind(first => NotEmpty(dto.LastName, "Nazwisko jest wymagane.")
                .Bind(last => NotEmpty(dto.Email, "Email jest wymagany.")
                    .Map(email => new Reader(
                        Guid.NewGuid(), first, last, email))))
            .Tap(readers.Add)
            .Tap(_ => unitOfWork.Save())
            .Map(reader => new ReaderResult(
                reader.Id, reader.FirstName, reader.LastName, reader.Email));

    public Result<LoanResult> Borrow(BorrowBookDto dto) =>
        FindBook(dto.BookId)
            .Bind(book => FindReader(dto.ReaderId)
                .Bind(reader => loans.GetActiveForBook(book.Id) is not null
                    ? Result.Failure<Reader>("Książka jest już wypożyczona.")
                    : reader.Blocked
                        ? Result.Failure<Reader>("Czytelnik jest zablokowany.")
                        : loans.GetActiveForReader(reader.Id) is not null
                            ? Result.Failure<Reader>(
                                "Czytelnik ma już aktywne wypożyczenie.")
                            : Result.Success(reader))
                .Bind(_ => Loan.Create(
                    book.Id, dto.ReaderId, dto.BorrowedOn, dto.DueOn)
                    .Tap(_ => book.MarkBorrowed())))
            .Tap(loans.Add)
            .Tap(_ => unitOfWork.Save())
            .Map(loan => new LoanResult(
                loan.Id, loan.BookId, loan.ReaderId, loan.DueOn));

    public Result<Unit> Return(ReturnBookDto dto) =>
        loans.Get(dto.LoanId) is { } loan
            ? loan.Return(dto.ReturnedOn)
                .Bind(_ => FindBook(loan.BookId))
                .Tap(book => book.MarkReturned())
                .Tap(_ => unitOfWork.Save())
                .Map(_ => Unit.Instance)
            : Result.Failure<Unit>("Wypożyczenie nie istnieje.");

    public Result<ReservationResult> Reserve(ReserveBookDto dto) =>
        FindBook(dto.BookId)
            .Bind(book => FindReader(dto.ReaderId)
                .Bind(_ => loans.GetActiveForBook(book.Id) is null
                    ? Result.Failure<Reservation>(
                        "Rezerwacja jest możliwa tylko dla wypożyczonej książki.")
                    : reservations.HasActive(book.Id, dto.ReaderId)
                        ? Result.Failure<Reservation>(
                            "Czytelnik ma już rezerwację tej książki.")
                        : Reservation.Create(book.Id, dto.ReaderId, dto.ReservedOn)))
            .Tap(reservations.Add)
            .Tap(_ => unitOfWork.Save())
            .Map(item => new ReservationResult(
                item.Id, item.BookId, item.ReaderId, item.ReservedOn));

    public Result<Unit> CancelReservation(CancelReservationDto dto) =>
        reservations.Get(dto.ReservationId) is { } reservation
            ? reservation.Cancel().Tap(_ => unitOfWork.Save())
            : Result.Failure<Unit>("Rezerwacja nie istnieje.");

    private Result<Book> FindBook(Guid id) =>
        books.Get(id) is { } book
            ? Result.Success(book)
            : Result.Failure<Book>("Książka nie istnieje.");

    private Result<Reader> FindReader(Guid id) =>
        readers.Get(id) is { } reader
            ? Result.Success(reader)
            : Result.Failure<Reader>("Czytelnik nie istnieje.");

    private static Result<string> NotEmpty(string value, string error) =>
        string.IsNullOrWhiteSpace(value)
            ? Result.Failure<string>(error)
            : Result.Success(value.Trim());
}

using Application.DomainServices;
using Application.Services;
using Application.UseCases.Dtos;
using Application.Utilities;
using Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

var builder = new ConfigurationBuilder();
builder.SetBasePath(AppContext.BaseDirectory)
    .AddJsonFile(
        "appsettings.json",
        optional: false,
        reloadOnChange: true);

// Skonfigurowanie połączenia z bazą danych
IConfiguration config = builder.Build();
var dbOptionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
dbOptionsBuilder.UseSqlite(config.GetConnectionString("default"));

// Stworzenie kontekstu
using var context = new AppDbContext(dbOptionsBuilder.Options);
context.Database.Migrate();

// Uruchomienie aplikacji
var bookRepository = new BookRepository(context);
var readerRepository = new ReaderRepository(context);
var loanRepository = new LoanRepository(context);
var reservationRepository = new ReservationRepository(context);
var unitOfWork = new UnitOfWork(context);

var libraryRules = new LibraryRules(loanRepository, reservationRepository);
var bookServices = new BookServices(
    bookRepository,
    unitOfWork,
    new BookUniquenessChecker(bookRepository),
    libraryRules);
var readerServices = new ReaderServices(
    readerRepository,
    unitOfWork,
    new ReaderUniquenessChecker(readerRepository),
    libraryRules);
var loanServices = new LoanServices(
    loanRepository,
    bookRepository,
    readerRepository,
    reservationRepository,
    unitOfWork,
    libraryRules);
var reservationServices = new ReservationServices(
    reservationRepository,
    bookRepository,
    readerRepository,
    unitOfWork,
    libraryRules);

var addBookResult = bookServices.AddBook(
    new AddBookDto("Pan Tadeusz", "Adam Mickiewicz", "9788373271890", 1));

addBookResult.Match(
    v => {
        Console.WriteLine($"Dodano książkę {v.Title} (id = {v.Id})");
        return Unit.Instance;
    },
    e => { Console.WriteLine(e); return Unit.Instance; }
);

var addAnnaResult = readerServices.AddReader(
    new AddReaderDto("Anna", "Nowak", "anna@example.com"));
var addJanResult = readerServices.AddReader(
    new AddReaderDto("Jan", "Kowalski", "jan@example.com"));

var borrowResult = addBookResult.Bind(book =>
    addAnnaResult.Bind(reader =>
        loanServices.BorrowBook(new BorrowBookDto(reader.Id, book.Id))));

borrowResult.Match(
    v => {
        Console.WriteLine($"Wypożyczono książkę, termin zwrotu: {v.DueDate}");
        return Unit.Instance;
    },
    e => { Console.WriteLine(e); return Unit.Instance; }
);

var borrowSecondResult = addBookResult.Bind(book =>
    addJanResult.Bind(reader =>
        loanServices.BorrowBook(new BorrowBookDto(reader.Id, book.Id))));

borrowSecondResult.Match(
    _ => { Console.WriteLine("Wypożyczono książkę."); return Unit.Instance; },
    e => { Console.WriteLine(e); return Unit.Instance; }
);

var reserveResult = addBookResult.Bind(book =>
    addJanResult.Bind(reader =>
        reservationServices.ReserveBook(new ReserveBookDto(reader.Id, book.Id))));

reserveResult.Match(
    _ => { Console.WriteLine("Dodano rezerwację."); return Unit.Instance; },
    e => { Console.WriteLine(e); return Unit.Instance; }
);

foreach (var book in context.Books)
{
    Console.WriteLine(
        $"{book.Title}: wolne egzemplarze {book.AvailableCopies}/{book.TotalCopies}");
}

using Application.DomainServices;
using Application.Services;
using Application.UseCases.Dtos;
using Application.Utilities;
using Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

Console.OutputEncoding = System.Text.Encoding.UTF8;

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
var clock = new SystemClock();
var lendingPolicy = new LendingPolicy(loanRepository, reservationRepository);

var bookServices = new BookServices(
    bookRepository, reservationRepository, lendingPolicy, unitOfWork);
var readerServices = new ReaderServices(
    readerRepository, reservationRepository, lendingPolicy, unitOfWork);
var lendingServices = new LendingServices(
    bookRepository, readerRepository, loanRepository, reservationRepository,
    lendingPolicy, clock, unitOfWork);
var queries = new LibraryQueries(context, clock);

Print("Błędny ISBN", bookServices.AddBook(
    new AddBookDto("978-83-000-0000-1", "Lalka", "Bolesław Prus", 1)));

// Książka o jednym egzemplarzu (ISBN jest losowy, by można było wielokrotnie uruchamiać aplikację).
var book = bookServices.AddBook(
    new AddBookDto(RandomIsbn13(), "Pan Tadeusz", "Adam Mickiewicz", 1));
Print("Dodanie książki", book);

var anna = readerServices.RegisterReader(new RegisterReaderDto("Anna", "Nowak"));
var piotr = readerServices.RegisterReader(new RegisterReaderDto("Piotr", "Wiśniewski"));
Print("Rejestracja czytelnika Anna", anna);
Print("Rejestracja czytelnika Piotr", piotr);

var bookId = book.Match(b => b.Id, _ => Guid.Empty);
var annaId = anna.Match(r => r.Id, _ => Guid.Empty);
var piotrId = piotr.Match(r => r.Id, _ => Guid.Empty);

var annaLoan = lendingServices.BorrowBook(new BorrowBookDto(annaId, bookId));
Print("Anna wypożycza książkę", annaLoan);
Print("Piotr wypożycza tę samą książkę",
    lendingServices.BorrowBook(new BorrowBookDto(piotrId, bookId)));
Print("Piotr rezerwuje książkę",
    lendingServices.ReserveBook(new ReserveBookDto(piotrId, bookId)));
Print("Anna przedłuża wypożyczenie",
    annaLoan.Bind(loan => lendingServices.ExtendLoan(loan.Id)));
Print("Usunięcie wypożyczonej książki", bookServices.RemoveBook(bookId));
Print("Anna zwraca książkę", annaLoan.Bind(loan => lendingServices.ReturnBook(loan.Id)));
Print("Anna ponownie wypożycza książkę",
    lendingServices.BorrowBook(new BorrowBookDto(annaId, bookId)));

var piotrLoan = lendingServices.BorrowBook(new BorrowBookDto(piotrId, bookId));
Print("Piotr wypożycza zarezerwowaną książkę", piotrLoan);
Print("Usunięcie czytelnika z wypożyczoną książką", readerServices.RemoveReader(piotrId));

Console.WriteLine("\nAktywne wypożyczenia Piotra:");
foreach (var loan in queries.GetActiveLoans(piotrId))
{
    Console.WriteLine($"  {loan.BookTitle} - termin zwrotu {loan.DueDate}");
}

Console.WriteLine("\nKatalog:");
foreach (var b in queries.GetBooks())
{
    Console.WriteLine($"  {b.Title} ({b.Author}), ISBN {b.Isbn}: dostępne {b.AvailableCopies}/{b.Copies}");
}

static void Print<T>(string operation, Result<T> result) =>
    Console.WriteLine(result.Match(
        _ => $"[OK]    {operation}",
        error => $"[BŁĄD]  {operation}: {error}"));

static string RandomIsbn13()
{
    var digits = "978" + string.Concat(Enumerable.Range(0, 9).Select(_ => Random.Shared.Next(10)));
    var sum = digits.Select((c, i) => (c - '0') * (i % 2 == 0 ? 1 : 3)).Sum();
    return digits + (10 - sum % 10) % 10;
}

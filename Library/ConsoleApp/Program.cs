using Application.Domain;
using Application.Queries.Dtos;
using Application.Services;
using Application.UseCases;
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

IConfiguration config = builder.Build();
var dbOptionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
dbOptionsBuilder.UseSqlite(config.GetConnectionString("default"));

using var context = new AppDbContext(dbOptionsBuilder.Options);
context.Database.Migrate();

var bookRepository = new BookRepository(context);
var readerRepository = new ReaderRepository(context);
var unitOfWork = new UnitOfWork(context);

var bookServices = new BookServices(bookRepository, unitOfWork);
var readerServices = new ReaderServices(readerRepository, unitOfWork);

IBookQueries bookQueries = new BookQueries(context);
IReaderQueries readerQueries = new ReaderQueries(context);

Console.WriteLine("Dodawanie czytelnika");
var addReaderDto = new AddReaderDto("Jan", "Kowalski", "jan.kowalski@example.com");
var addReaderResult = readerServices.AddReader(addReaderDto);

Guid readerId = Guid.Empty;
addReaderResult.Match(
    v => {
        readerId = v.Id;
        Console.WriteLine($"Dodano czytelnika: {v.Name.FirstName} {v.Name.LastName} ({v.Email.Value}) [id = {v.Id}]");
        return Unit.Instance;
    },
    e => { Console.WriteLine($"Błąd: {e}"); return Unit.Instance; }
);

Console.WriteLine("\nDodawanie książki");
var addBookDto = new AddBookDto("Agatha", "Christie", "4.50 z Paddington", 3);
var addBookResult = bookServices.AddBook(addBookDto);

Guid bookId = Guid.Empty;
addBookResult.Match(
    v => {
        bookId = v.Id;
        Console.WriteLine($"Dodano książku: '{v.BookTitle.Value}' autora {v.Author.Name.FirstName} {v.Author.Name.LastName}, egzemplarzy: {v.BookCopies.Value} [id = {v.Id}]");
        return Unit.Instance;
    },
    e => { Console.WriteLine($"Błąd: {e}"); return Unit.Instance; }
);

Console.WriteLine("\nLista książek");
foreach (var book in bookQueries.GetAll())
{
    Console.WriteLine($"Tytuł: '{book.Title}', Autor: {book.AuthorFirstName} {book.AuthorLastName}, Dostępne sztuki: {book.AvailableCopies}");
}

if (bookId != Guid.Empty && readerId != Guid.Empty)
{
    Console.WriteLine("\nWypożyczenie książki");
    var borrowDto = new BorrowBookDto(bookId, readerId);
    var borrowResult = bookServices.BorrowBook(borrowDto);

    borrowResult.Match(
        _ => { Console.WriteLine("Książka została wypożyczona!"); return Unit.Instance; },
        e => { Console.WriteLine($"Nie udało się wypożyczyć: {e}"); return Unit.Instance; }
    );

    var updatedBook = bookQueries.GetById(bookId);
    updatedBook.Match(
        b => { Console.WriteLine($"Aktualna liczba dostępnych sztuk po wypożyczeniu: {b.AvailableCopies}"); return Unit.Instance; },
        () => { Console.WriteLine("Nie znaleziono książki."); return Unit.Instance; }
    );

    Console.WriteLine("\nZwrot książki");
    var returnDto = new ReturnBookDto(bookId);
    var returnResult = bookServices.ReturnBook(returnDto);

    returnResult.Match(
        _ => { Console.WriteLine("Książka została zwrócona!"); return Unit.Instance; },
        e => { Console.WriteLine($"Nie udało się zwrócić: {e}"); return Unit.Instance; }
    );
}

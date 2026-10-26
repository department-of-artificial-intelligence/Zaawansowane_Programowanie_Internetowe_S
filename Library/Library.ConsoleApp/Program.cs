using Library.Application.Services;
using Library.Application.UseCases.Dtos;
using Library.Application.Utilities;
using Library.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

var builder = new ConfigurationBuilder();
builder.SetBasePath(AppContext.BaseDirectory).AddJsonFile("appsettings.json", optional: false);

IConfiguration config = builder.Build();
var dbOptions = new DbContextOptionsBuilder<LibraryDbContext>()
    .UseSqlite(config.GetConnectionString("default"))
    .Options;

using var context = new LibraryDbContext(dbOptions);
context.Database.Migrate();

var bookRepo = new BookRepository(context);
var readerRepo = new ReaderRepository(context);
var loanRepo = new LoanRepository(context);
var uow = new UnitOfWork(context);
var service = new LibraryServices(bookRepo, readerRepo, loanRepo, uow);

// 1. Rejestracja czytelnika
var readerId = service.RegisterReader(new AddReaderDto("Milosz Jakubczak", "milosz@example.com"))
    .Match(id => id, err => throw new Exception(err));
Console.WriteLine($"[OK] Zarejestrowano czytelnika ID: {readerId}");

// 2. Dodanie ksiazki
var bookId = service.AddBook(new AddBookDto("Czysta Architektura", "Robert C. Martin", "9788328342415"))
    .Match(id => id, err => throw new Exception(err));
Console.WriteLine($"[OK] Dodano ksiazke ID: {bookId}");

// 3. Wypozyczenie ksiazki
service.BorrowBook(new BorrowBookDto(bookId, readerId)).Match(
    _ => { Console.WriteLine("[OK] Ksiazka zostala wypozyczona."); return Unit.Instance; },
    err => { Console.WriteLine($"[Blad wypozyczenia]: {err}"); return Unit.Instance; }
);

// 4. Proba ponownego wypozyczenia tej samej ksiazki (test reguly biznesowej)
service.BorrowBook(new BorrowBookDto(bookId, readerId)).Match(
    _ => { Console.WriteLine("[BLAD LOGIKI] Udalo sie wypozyczyc zajeta pozycje!"); return Unit.Instance; },
    err => { Console.WriteLine($"[OK] Poprawnie zablokowano ponowne wypozyczenie: {err}"); return Unit.Instance; }
);

// 5. Zwrot ksiazki
service.ReturnBook(new ReturnBookDto(bookId)).Match(
    _ => { Console.WriteLine("[OK] Ksiazka zwrocona pomyslnie."); return Unit.Instance; },
    err => { Console.WriteLine($"[Blad zwrotu]: {err}"); return Unit.Instance; }
);
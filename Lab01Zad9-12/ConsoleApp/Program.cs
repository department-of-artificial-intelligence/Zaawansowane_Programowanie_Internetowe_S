using Application.Domain;
using Application.Services;
using Application.UseCases.Dtos;
using Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;

var builder = new ConfigurationBuilder()
    .SetBasePath(AppContext.BaseDirectory)
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

var config = builder.Build();
var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
optionsBuilder.UseSqlite(config.GetConnectionString("default"));

using var context = new AppDbContext(optionsBuilder.Options);

context.Database.EnsureDeleted();
context.Database.Migrate();

var bookRepository = new BookRepository(context);
var readerRepository = new ReaderRepository(context);
var unitOfWork = new UnitOfWork(context);
var libraryService = new LibraryService(bookRepository, readerRepository, unitOfWork);


var isbn = Isbn.Create("9781234567897").Match(i => i, e => throw new Exception(e));
var bookResult = Book.Create(isbn, "Wiedźmin");
var readerResult = Reader.Create("jan.kowalski@example.com");

var book = bookResult.Match(b => b, e => throw new Exception(e));
var reader = readerResult.Match(r => r, e => throw new Exception(e));

bookRepository.Add(book);
readerRepository.Add(reader);
unitOfWork.Save();

Console.WriteLine($"Zarejestrowano czytelnika: {reader.Email} (ID: {reader.Id})");
Console.WriteLine($"Dodano książkę: {book.Title} (ID: {book.Id})");

Console.WriteLine("\nPróba wypożyczenia");
var borrowDto = new BorrowBookDto(reader.Id, book.Id);
var borrowResult = libraryService.BorrowBook(borrowDto);

borrowResult.Match(
    _ => { Console.WriteLine("Udało się wypożyczyć książkę"); return true; },
    fail => { Console.WriteLine($"Błąd: {fail}"); return false; }
);

Console.WriteLine("\nPróba wypożyczenia tej samej książki");
var borrowAgainResult = libraryService.BorrowBook(borrowDto);

borrowAgainResult.Match(
    _ => { Console.WriteLine("Udało się wypożyczyć książkę"); return true; },
    fail => { Console.WriteLine($"Błąd: {fail}"); return false; }
);
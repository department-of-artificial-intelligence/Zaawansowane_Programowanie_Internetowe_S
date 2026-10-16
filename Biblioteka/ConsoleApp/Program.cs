using Application.Domain;
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

// 1. Skonfigurowanie połączenia z bazą danych
IConfiguration config = builder.Build();
var dbOptionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
dbOptionsBuilder.UseSqlite(config.GetConnectionString("default"));

// 2. Stworzenie kontekstu i migracja
using var context = new AppDbContext(dbOptionsBuilder.Options);
context.Database.Migrate();

// 3. Inicjalizacja repozytoriów, usług i jednostki pracy
var contactRepository = new ContactRepository(context);
var categoryRepository = new CategoryRepository(context);
var unitOfWork = new UnitOfWork(context);
var categoryUniquenessChecker = new CategoryUniquenessChecker(categoryRepository);

var categoryServices = new CategoryServices(
    categoryRepository, contactRepository, unitOfWork, categoryUniquenessChecker);

var contactServices = new ContactServices(
    contactRepository, categoryRepository, unitOfWork);

// 4. Zadanie 1: Dodanie nowego kontaktu (z obiektami wartości FirstName i LastName)
var addContactDto = new AddContactDto("Jan", "Kowalski", Sex.Male, 30);
var addContactResult = contactServices.AddContact(addContactDto);

addContactResult.Match(
    v => {
        Console.WriteLine($"1. Dodano kontakt {v.FirstName} {v.LastName} (id = {v.Id})");
        return Unit.Instance;
    },
    e => { Console.WriteLine($"Błąd dodawania kontaktu: {e}"); return Unit.Instance; }
);

// 5. Pobranie lub stworzenie kategorii (zabezpieczenie przed duplikatem)
Guid categoryId;
var existingCategory = categoryRepository.GetByName("Praca");
if (existingCategory is Option<Category>.Some someCat)
{
    categoryId = someCat.Value.Id;
}
else
{
    var catResult = categoryServices.AddCategory(new AddCategoryDto("Praca"));
    categoryId = catResult.Match(c => c.Id, _ => Guid.Empty);
}

// 6. Zadanie 2: Dodanie adresu e-mail do kontaktu (z walidacją Regex)
addContactResult.Tap(contact =>
{
    var addEmailDto = new AddEmailDto(contact.Id, "jan.kowalski@example.com", categoryId);
    var addEmailResult = contactServices.AddEmail(addEmailDto);

    addEmailResult.Match(
        _ => { Console.WriteLine("2. Dodano e-mail do kontaktu."); return Unit.Instance; },
        e => { Console.WriteLine($"Błąd dodawania e-maila: {e}"); return Unit.Instance; }
    );
});

// 7. Zadanie 6: Usunięcie adresu e-mail z kontaktu
addContactResult.Tap(contact =>
{
    var removeEmailDto = new RemoveEmailDto(contact.Id, "jan.kowalski@example.com");
    var removeEmailResult = contactServices.RemoveEmail(removeEmailDto);

    removeEmailResult.Match(
        _ => { Console.WriteLine("3. Usunięto adres e-mail z kontaktu."); return Unit.Instance; },
        err => { Console.WriteLine($"Błąd usuwania adresu e-mail: {err}"); return Unit.Instance; }
    );
});

// 8. Zadanie 5: Edycja danych kontaktu
addContactResult.Tap(contact =>
{
    var updateContactDto = new UpdateContactDto(
        contact.Id, "Janusz", "Kowalski-Nowak", Sex.Male, 35);

    var updateResult = contactServices.UpdateContact(updateContactDto);
    updateResult.Match(
        _ => { Console.WriteLine("4. Zaktualizowano dane kontaktu."); return Unit.Instance; },
        err => { Console.WriteLine($"Błąd edycji kontaktu: {err}"); return Unit.Instance; }
    );
});

// 9. Zadanie 7: Dołączanie i usuwanie numeru telefonu
addContactResult.Tap(contact =>
{
    // A. Dodanie numeru telefonu
    var addPhoneDto = new AddPhoneDto(contact.Id, "+48123456789", categoryId);
    var addPhoneResult = contactServices.AddPhone(addPhoneDto);

    addPhoneResult.Match(
        _ => { Console.WriteLine("5a. Dodano numer telefonu do kontaktu."); return Unit.Instance; },
        err => { Console.WriteLine($"Błąd dodawania telefonu: {err}"); return Unit.Instance; }
    );

    // B. Usunięcie numeru telefonu
    var removePhoneDto = new RemovePhoneDto(contact.Id, "+48123456789");
    var removePhoneResult = contactServices.RemovePhone(removePhoneDto);

    removePhoneResult.Match(
        _ => { Console.WriteLine("5b. Usunięto numer telefonu z kontaktu."); return Unit.Instance; },
        err => { Console.WriteLine($"Błąd usuwania telefonu: {err}"); return Unit.Instance; }
    );
});

// 10. Zadanie 8: Zapisywanie i usuwanie ważnych dat związanych z kontaktem
addContactResult.Tap(contact =>
{
    // A. Dodanie ważnej daty
    var addDateDto = new AddImportantDateDto(
        contact.Id,
        new DateOnly(1995, 5, 20),
        "Urodziny");

    var addDateResult = contactServices.AddImportantDate(addDateDto);
    addDateResult.Match(
        _ => { Console.WriteLine("6a. Dodano ważną datę do kontaktu."); return Unit.Instance; },
        err => { Console.WriteLine($"Błąd dodawania daty: {err}"); return Unit.Instance; }
    );

    // B. Usunięcie ważnej daty
    var removeDateDto = new RemoveImportantDateDto(contact.Id, "Urodziny");
    var removeDateResult = contactServices.RemoveImportantDate(removeDateDto);

    removeDateResult.Match(
        _ => { Console.WriteLine("6b. Usunięto ważną datę z kontaktu."); return Unit.Instance; },
        err => { Console.WriteLine($"Błąd usuwania daty: {err}"); return Unit.Instance; }
    );
});

// 11. Zadanie 5: Usunięcie kontaktu (zawsze na samym końcu operacji testowych)
addContactResult.Tap(contact =>
{
    var removeResult = contactServices.RemoveContact(contact.Id);
    removeResult.Match(
        _ => { Console.WriteLine("7. Usunięto kontakt."); return Unit.Instance; },
        err => { Console.WriteLine($"Błąd usuwania kontaktu: {err}"); return Unit.Instance; }
    );
});

// 12. Prezentacja listy kategorii
Console.WriteLine("\nAktualna lista kategorii w bazie:");
foreach (var cat in context.Categories)
{
    Console.WriteLine($"- {cat.Name}");
}

// Inicjalizacja nowych modułów bibliotecznych
var bookRepository = new BookRepository(context);
var readerRepository = new ReaderRepository(context);
var loanRepository = new LoanRepository(context);

var bookServices = new BookServices(bookRepository, unitOfWork);
var readerServices = new ReaderServices(readerRepository, unitOfWork);
var loanServices = new LoanServices(loanRepository, bookRepository, readerRepository, unitOfWork);

Console.WriteLine("\n--- TEST MODUŁU BIBLIOTEKI ---");

// 1. Rejestracja czytelnika
var readerCard = $"CARD-{Random.Shared.Next(10000, 99999)}";
var registerDto = new RegisterReaderDto("Adam", "Nowak", "adam.nowak@example.com", readerCard);
var readerResult = readerServices.RegisterReader(registerDto);

readerResult.Match(
  reader =>
  {
      Console.WriteLine($"Zarejestrowano czytelnika: {reader.FullName} [Karta: {reader.CardNumber}]");

      // 2. Dodanie książki z 2 egzemplarzami fizycznymi
      var copyCode1 = $"LIB-{Random.Shared.Next(1000, 4999)}";
      var copyCode2 = $"LIB-{Random.Shared.Next(5000, 9999)}";

      var addBookDto = new AddBookDto(
        "Domain-Driven Design",
        "Eric Evans",
        "978-0321125217",
        [copyCode1, copyCode2]);

      var bookResult = bookServices.AddBook(addBookDto);
      bookResult.Tap(book =>
      {
          Console.WriteLine($"Dodano książkę: '{book.Title}' z {book.CopiesCount} egzemplarzami ({copyCode1}, {copyCode2}).");

          // 3. Wypożyczenie pierwszego egzemplarza
          var borrowResult = loanServices.BorrowCopy(new BorrowBookCopyDto(reader.ReaderId, copyCode1, DateOnly.FromDateTime(DateTime.Today)));
          borrowResult.Match(
            loan =>
            {
                Console.WriteLine($"Wypożyczono egzemplarz {loan.CopyCode}. Termin zwrotu: {loan.DueDate}.");

                // 4. Próba ponownego wypożyczenia tego samego egzemplarza (weryfikacja niezmiennika)
                var invalidBorrow = loanServices.BorrowCopy(new BorrowBookCopyDto(reader.ReaderId, copyCode1, DateOnly.FromDateTime(DateTime.Today)));
                invalidBorrow.Match(
              _ => Unit.Instance,
              err => { Console.WriteLine($"Prawidłowo zablokowano ponowne wypożyczenie: {err}"); return Unit.Instance; }
            );

                // 5. Przedłużenie terminu (prolongata)
                var extendResult = loanServices.ExtendLoan(new ExtendLoanDto(loan.LoanId, 14));
                extendResult.Match(
              _ => { Console.WriteLine("Przedłużono termin zwrotu o 14 dni."); return Unit.Instance; },
              err => { Console.WriteLine($"Błąd przedłużenia: {err}"); return Unit.Instance; }
            );

                // 6. Zwrot egzemplarza do biblioteki
                var returnResult = loanServices.ReturnCopy(new ReturnBookCopyDto(copyCode1, DateOnly.FromDateTime(DateTime.Today)));
                returnResult.Match(
              _ => { Console.WriteLine($"Zwrócono egzemplarz {copyCode1} – status przywrócony do dostępnego."); return Unit.Instance; },
              err => { Console.WriteLine($"Błąd zwrotu: {err}"); return Unit.Instance; }
            );
                return Unit.Instance;
            },
            err => { Console.WriteLine($"Błąd wypożyczenia: {err}"); return Unit.Instance; }
          );
      });

      return Unit.Instance;
  },
  err => { Console.WriteLine($"Błąd rejestracji czytelnika: {err}"); return Unit.Instance; }
);
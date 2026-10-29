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
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

// Skonfigurowanie połączenia z bazą danych
IConfiguration config = builder.Build();
var dbOptionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
dbOptionsBuilder.UseSqlite(config.GetConnectionString("default"));

// Stworzenie kontekstu
using var context = new AppDbContext(dbOptionsBuilder.Options);
context.Database.Migrate();

// REPOZYTORIA I SERWISY
var categoryRepository = new CategoryRepository(context);
var contactRepository = new ContactRepository(context);
var unitOfWork = new UnitOfWork(context);

var categoryUniquenessChecker = new CategoryUniquenessChecker(categoryRepository);

var categoryServices = new CategoryServices(
    categoryRepository, contactRepository, unitOfWork, categoryUniquenessChecker);

var contactServices = new ContactServices(
    contactRepository, categoryRepository, unitOfWork);

// --- TEST ZADAŃ 1-3 ---
var addContactDto = new AddContactDto("Jan", "Kowalski", Sex.Male, 30);
var addContactResult = contactServices.AddContact(addContactDto);

addContactResult.Match(
    v =>
    {
        Console.WriteLine($"Dodano kontakt {v.FirstName} {v.LastName} (id = {v.Id})");
        return Unit.Instance;
    },
    e => { Console.WriteLine(e); return Unit.Instance; }
);

// Pobranie istniejącej kategorii "Praca" lub "Praca / Biuro"
var addEmailResult = addContactResult.Bind(contact =>
{
    var categoryOption = categoryRepository.GetByName("Praca");

    return categoryOption.Match(
        category =>
        {
            var addEmailDto = new AddEmailDto(contact.Id, "jan.kowalski@example.com", category.Id);
            return contactServices.AddEmail(addEmailDto);
        },
        () => categoryServices.AddCategory(new AddCategoryDto("Praca"))
            .Bind(category =>
            {
                var addEmailDto = new AddEmailDto(contact.Id, "jan.kowalski@example.com", category.Id);
                return contactServices.AddEmail(addEmailDto);
            })
    );
});

addEmailResult.Match(
    _ => { Console.WriteLine("Dodano e-mail."); return Unit.Instance; },
    e => { Console.WriteLine(e); return Unit.Instance; }
);

// Modyfikacja kategorii "Praca" na "Praca / Biuro"
var categoryToUpdate = categoryRepository.GetByName("Praca");

categoryToUpdate.Match(
    category =>
    {
        var updateResult = categoryServices.UpdateCategory(category.Id, "Praca / Biuro");
        updateResult.Match(
            _ => { Console.WriteLine("Pomyślnie zmodyfikowano nazwę kategorii na 'Praca / Biuro'."); return Unit.Instance; },
            err => { Console.WriteLine($"Błąd podczas modyfikacji: {err}"); return Unit.Instance; }
        );
        return Unit.Instance;
    },
    () => { Console.WriteLine("Kategoria 'Praca' została już zmieniona lub nie istnieje."); return Unit.Instance; }
);

// --- TEST ZADANIA 4: Usuwanie kategorii (Blokada) ---
Console.WriteLine("\n--- TEST ZADANIA 4: Usuwanie kategorii ---");

// Szukamy zmienionej kategorii "Praca / Biuro", a jeśli jej nie ma, to "Praca"
var categoryToTest = categoryRepository.GetByName("Praca / Biuro");

categoryToTest.Match(
    category =>
    {
        var removeResult = categoryServices.RemoveCategory(category.Id);
        removeResult.Match(
            _ => { Console.WriteLine("Sukces: Usunięto kategorię."); return Unit.Instance; },
            err => { Console.WriteLine($"Blokada działa poprawnie: {err}"); return Unit.Instance; }
        );
        return Unit.Instance;
    },
    () =>
    {
        // Jeśli nie znaleziono pod nową nazwą, próba wyszukania starej nazwy
        categoryRepository.GetByName("Praca").Match(
            category =>
            {
                var removeResult = categoryServices.RemoveCategory(category.Id);
                removeResult.Match(
                    _ => { Console.WriteLine("Sukces: Usunięto kategorię."); return Unit.Instance; },
                    err => { Console.WriteLine($"Blokada działa poprawnie: {err}"); return Unit.Instance; }
                );
                return Unit.Instance;
            },
            () => { Console.WriteLine("Nie znaleziono kategorii do testu usunięcia."); return Unit.Instance; }
        );
        return Unit.Instance;
    }
);


// --- TEST ZADANIA 5: Edycja i usuwanie kontaktu ---
Console.WriteLine("\n--- TEST ZADANIA 5: Edycja i usuwanie kontaktu ---");

var firstContact = contactRepository.GetAll().FirstOrDefault();

if (firstContact != null)
{
    // 1. Modyfikacja kontaktu
    var updateDto = new UpdateContactDto("Janusz", "Kowalski", Sex.Male, 35);
    var updateResult = contactServices.UpdateContact(firstContact.Id, updateDto);

    updateResult.Match(
        _ => { Console.WriteLine($"Zmieniono imię kontaktu na Janusz (Wiek: 35)!"); return Unit.Instance; },
        err => { Console.WriteLine($"Błąd podczas edycji kontaktu: {err}"); return Unit.Instance; }
    );

    // 2. Usuwanie kontaktu
    var removeContactResult = contactServices.RemoveContact(firstContact.Id);

    removeContactResult.Match(
        _ => { Console.WriteLine("Pomyślnie usunięto kontakt!"); return Unit.Instance; },
        err => { Console.WriteLine($"Błąd podczas usuwania kontaktu: {err}"); return Unit.Instance; }
    );
}
else
{
    Console.WriteLine("Brak kontaktów do przetestowania edycji i usuwania.");
}


// --- TEST ZADANIA 6: Usuwanie adresu e-mail ---
Console.WriteLine("\n--- TEST ZADANIA 6: Usuwanie e-maila ---");

var contactToTestEmail = contactRepository.GetAll().FirstOrDefault();

if (contactToTestEmail != null)
{
    var removeEmailDto = new RemoveEmailDto(contactToTestEmail.Id, "jan.kowalski@example.com");
    var removeEmailResult = contactServices.RemoveEmail(removeEmailDto);

    removeEmailResult.Match(
        _ => { Console.WriteLine("Pomyślnie usunięto adres e-mail z kontaktu!"); return Unit.Instance; },
        err => { Console.WriteLine($"Informacja / Błąd podczas usuwania e-maila: {err}"); return Unit.Instance; }
    );
}


// --- TEST ZADANIA 7: Dodawanie i usuwanie numeru telefonu ---
Console.WriteLine("\n--- TEST ZADANIA 7: Dodawanie i usuwanie telefonu ---");

var contactForPhoneTest = contactRepository.GetAll().FirstOrDefault();
var defaultCategory = categoryRepository.GetAll().FirstOrDefault();

if (contactForPhoneTest != null && defaultCategory != null)
{
    // 1. Dodawanie telefonu
    var addPhoneDto = new AddPhoneDto(contactForPhoneTest.Id, "+48 123 456 789", defaultCategory.Id);
    var addPhoneResult = contactServices.AddPhone(addPhoneDto);

    addPhoneResult.Match(
        _ => { Console.WriteLine("Pomyślnie dodano numer telefonu do kontaktu!"); return Unit.Instance; },
        err => { Console.WriteLine($"Błąd podczas dodawania telefonu: {err}"); return Unit.Instance; }
    );

    // 2. Usuwanie telefonu
    var removePhoneDto = new RemovePhoneDto(contactForPhoneTest.Id, "+48 123 456 789");
    var removePhoneResult = contactServices.RemovePhone(removePhoneDto);

    removePhoneResult.Match(
        _ => { Console.WriteLine("Pomyślnie usunięto numer telefonu z kontaktu!"); return Unit.Instance; },
        err => { Console.WriteLine($"Błąd podczas usuwania telefonu: {err}"); return Unit.Instance; }
    );

    var bookRepository = new BookRepository(context);
    var readerRepository = new ReaderRepository(context);
    var loanRepository = new LoanRepository(context);

    var libraryServices = new LibraryServices(
        bookRepository,
        readerRepository,
        loanRepository,
        unitOfWork
    );
    context.Database.Migrate();

    // --- TEST ZADAŃ 9-12: System Biblioteczny ---
    Console.WriteLine("\n--- TEST ZADAŃ 9-12: System Biblioteczny ---");

    // 2. Test dodawania książki
    var addBookResult = libraryServices.AddBook(
        new AddBookDto("Wiedźmin: Ostatnie Życzenie", "Andrzej Sapkowski", "978-83-7578-063-5")
    );

    // 3. Test rejestracji czytelnika
    var registerReaderResult = libraryServices.RegisterReader(
        new RegisterReaderDto("Jan", "Kowalski", "jan.kowalski@example.com")
    );

    // 4. Przeprowadzenie procesu wypożyczenia
    addBookResult.Bind(book => registerReaderResult.Bind(reader =>
    {
        Console.WriteLine($"[Sukces] Dodano książkę: '{book.Title}' (ID: {book.BookId})");
        Console.WriteLine($"[Sukces] Zarejestrowano czytelnika: '{reader.FullName}' (ID: {reader.ReaderId})");

        // Próba wypożyczenia książki przez czytelnika
        return libraryServices.BorrowBook(new BorrowBookDto(book.BookId, reader.ReaderId));
    }))
    .Match(
        _ => { Console.WriteLine("-> Pomyślnie wypożyczono książkę!"); return Unit.Instance; },
        err => { Console.WriteLine($"-> Błąd wypożyczenia: {err}"); return Unit.Instance; }
    );
}



Console.WriteLine("\nWszystkie kategorie w bazie:");
foreach (var category in context.Categories)
{
    Console.WriteLine($"- {category.Name}");
}
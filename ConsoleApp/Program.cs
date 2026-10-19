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

// Skonfigurowanie połączenia z bazą danych
IConfiguration config = builder.Build();
var dbOptionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
dbOptionsBuilder.UseSqlite(config.GetConnectionString("default"));

// Stworzenie kontekstu
using var context = new AppDbContext(dbOptionsBuilder.Options);
context.Database.Migrate();

// Utworzenie adapterów, usług domeny i usług aplikacji
var categoryRepository = new CategoryRepository(context);
var contactRepository = new ContactRepository(context);
var unitOfWork = new UnitOfWork(context);

var categoryUniquenessChecker = new CategoryUniquenessChecker(categoryRepository);
var categoryUsageChecker = new CategoryUsageChecker(contactRepository);

var categoryServices = new CategoryServices(
    categoryRepository, unitOfWork, categoryUniquenessChecker, categoryUsageChecker);
var contactServices = new ContactServices(
    contactRepository, categoryRepository, unitOfWork);

var categoryQueries = new CategoryQueries(context);

// Pomocnicza funkcja do wypisywania wyników
static void Print(string label, Result<Unit> result) =>
    result.Match(
        _ => { Console.WriteLine($"{label}: OK"); return Unit.Instance; },
        e => { Console.WriteLine($"{label}: {e}"); return Unit.Instance; });

// =====================================================================
// Kod z instrukcji: kontakt, kategoria, e-mail
// =====================================================================
Console.WriteLine("--- Kod z instrukcji ---");

var addContactResult = contactServices.AddContact(
    new AddContactDto("Jan", "Kowalski", Sex.Male, 30));

addContactResult.Match(
    v =>
    {
        Console.WriteLine(
            $"Dodano kontakt {v.FirstName} {v.LastName} (id = {v.Id})");
        return Unit.Instance;
    },
    e => { Console.WriteLine(e); return Unit.Instance; });

var addEmailResult = addContactResult.Bind(contact =>
    categoryServices.AddCategory(new AddCategoryDto("Praca"))
        .Bind(category => contactServices.AddEmail(
            new AddEmailDto(contact.Id, "jan.kowalski@example.com", category.Id))));

Print("Dodanie e-maila", addEmailResult);

// =====================================================================
// Zadanie 1: obiekty wartości FirstName i LastName
// =====================================================================
Console.WriteLine("--- Test zadania 1 ---");

Print("Kontakt z pustym imieniem (oczekiwany błąd)",
    contactServices
        .AddContact(new AddContactDto("", "Kowalski", Sex.Male, 30))
        .Map(_ => Unit.Instance));

Print("Kontakt ze zbyt długim nazwiskiem (oczekiwany błąd)",
    contactServices
        .AddContact(new AddContactDto("Jan", new string('x', 101), Sex.Male, 30))
        .Map(_ => Unit.Instance));

// =====================================================================
// Zadanie 2: walidacja formatu adresu e-mail
// =====================================================================
Console.WriteLine("--- Test zadania 2 ---");

var emailTestCategory = categoryServices
    .AddCategory(new AddCategoryDto($"Email-{Guid.NewGuid():N}"));

var emailTestContact = contactServices
    .AddContact(new AddContactDto("Piotr", "Zieliński", Sex.Male, 40));

Print("Niepoprawny adres e-mail (oczekiwany błąd)",
    emailTestContact.Bind(contact => emailTestCategory.Bind(category =>
        contactServices.AddEmail(
            new AddEmailDto(contact.Id, "piotr.zielinski", category.Id)))));

Print("Poprawny adres e-mail",
    emailTestContact.Bind(contact => emailTestCategory.Bind(category =>
        contactServices.AddEmail(
            new AddEmailDto(contact.Id, "Piotr.Zielinski@Example.com", category.Id)))));

Print("Ten sam adres innymi literami (oczekiwany błąd)",
    emailTestContact.Bind(contact => emailTestCategory.Bind(category =>
        contactServices.AddEmail(
            new AddEmailDto(contact.Id, "piotr.zielinski@example.com", category.Id)))));

// =====================================================================
// Zadanie 3: zmiana nazwy kategorii
// =====================================================================
Console.WriteLine("--- Test zadania 3 ---");

var renameTest = categoryServices
    .AddCategory(new AddCategoryDto($"DoZmiany-{Guid.NewGuid():N}"));

Print("Zmiana nazwy kategorii",
    renameTest.Bind(category => categoryServices.RenameCategory(
        new RenameCategoryDto(category.Id, $"Zmieniona-{Guid.NewGuid():N}"))));

Print("Zmiana nazwy na pustą (oczekiwany błąd)",
    renameTest.Bind(category => categoryServices.RenameCategory(
        new RenameCategoryDto(category.Id, ""))));

Print("Zmiana nazwy na istniejącą 'Praca' (oczekiwany błąd)",
    renameTest.Bind(category => categoryServices.RenameCategory(
        new RenameCategoryDto(category.Id, "Praca"))));

// =====================================================================
// Zadania 4-6: usuwanie kategorii, edycja/usuwanie kontaktu, usuwanie e-maila
// =====================================================================
Console.WriteLine("--- Test zadań 4-6 ---");

var setup = contactServices
    .AddContact(new AddContactDto("Anna", "Nowak", Sex.Female, 25))
    .Bind(contact => categoryServices
        .AddCategory(new AddCategoryDto($"Test-{Guid.NewGuid():N}"))
        .Bind(category => contactServices
            .AddEmail(new AddEmailDto(contact.Id, "anna@example.com", category.Id))
            .Map(_ => (contact, category))));

setup.Match(
    t =>
    {
        var (contact, category) = t;

        Print("Zad. 4: usunięcie używanej kategorii (oczekiwany błąd)",
            categoryServices.RemoveCategory(category.Id));
        Print("Zad. 4: usunięcie nieistniejącej kategorii (oczekiwany błąd)",
            categoryServices.RemoveCategory(Guid.NewGuid()));

        Print("Zad. 5: edycja kontaktu",
            contactServices.UpdateContact(
                new UpdateContactDto(contact.Id, "Anna", "Kowalska", Sex.Female, 26)));
        Print("Zad. 5: edycja z błędnym wiekiem (oczekiwany błąd)",
            contactServices.UpdateContact(
                new UpdateContactDto(contact.Id, "Anna", "Kowalska", Sex.Female, 5)));

        Print("Zad. 6: usunięcie e-maila",
            contactServices.RemoveEmail(
                new RemoveEmailDto(contact.Id, "anna@example.com")));
        Print("Zad. 6: ponowne usunięcie tego samego e-maila (oczekiwany błąd)",
            contactServices.RemoveEmail(
                new RemoveEmailDto(contact.Id, "anna@example.com")));

        Print("Zad. 4: usunięcie kategorii, która nie jest już używana",
            categoryServices.RemoveCategory(category.Id));

        Print("Zad. 5: usunięcie kontaktu",
            contactServices.RemoveContact(contact.Id));
        Print("Zad. 5: ponowne usunięcie kontaktu (oczekiwany błąd)",
            contactServices.RemoveContact(contact.Id));

        return Unit.Instance;
    },
    e => { Console.WriteLine(e); return Unit.Instance; });
// =====================================================================
// Zadania 7-8: telefony i ważne daty
// =====================================================================
Console.WriteLine("--- Test zadań 7-8 ---");

var contact78 = contactServices.AddContact(
    new AddContactDto("Marek", "Wiśniewski", Sex.Male, 35));

contact78.Match(
    c78 =>
    {
        Print("Zad. 7: dodanie telefonu",
            contactServices.AddPhone(
                new AddPhoneDto(c78.Id, "+48 600 100 200", PhoneType.Mobile)));
        Print("Zad. 7: ten sam numer w innym formacie (oczekiwany błąd)",
            contactServices.AddPhone(
                new AddPhoneDto(c78.Id, "+48-600-100-200", PhoneType.Work)));
        Print("Zad. 7: niepoprawny numer (oczekiwany błąd)",
            contactServices.AddPhone(
                new AddPhoneDto(c78.Id, "abc123", PhoneType.Home)));
        Print("Zad. 7: drugi telefon",
            contactServices.AddPhone(
                new AddPhoneDto(c78.Id, "22 123 45 67", PhoneType.Home)));
        Print("Zad. 7: usunięcie telefonu",
            contactServices.RemovePhone(
                new RemovePhoneDto(c78.Id, "+48600100200")));

        Print("Zad. 8: dodanie urodzin",
            contactServices.AddImportantDate(
                new AddImportantDateDto(c78.Id, new DateOnly(1990, 5, 17), "Urodziny")));
        Print("Zad. 8: drugie 'urodziny' (oczekiwany błąd)",
            contactServices.AddImportantDate(
                new AddImportantDateDto(c78.Id, new DateOnly(1991, 1, 1), "urodziny")));
        Print("Zad. 8: data sprzed 1900 (oczekiwany błąd)",
            contactServices.AddImportantDate(
                new AddImportantDateDto(c78.Id, new DateOnly(1850, 1, 1), "Rocznica")));
        Print("Zad. 8: dodanie rocznicy ślubu",
            contactServices.AddImportantDate(
                new AddImportantDateDto(c78.Id, new DateOnly(2015, 8, 22), "Rocznica ślubu")));
        Print("Zad. 8: usunięcie urodzin",
            contactServices.RemoveImportantDate(
                new RemoveImportantDateDto(c78.Id, "Urodziny")));

        // Podgląd zapisanych danych
        contactRepository.GetById(c78.Id).Match(
            loaded =>
            {
                Console.WriteLine($"Kontakt: {loaded.FirstName.Value} {loaded.LastName.Value}");
                foreach (var phone in loaded.Phones)
                    Console.WriteLine($"  Telefon: {phone.Number.Value} ({phone.Type})");
                foreach (var importantDate in loaded.ImportantDates)
                    Console.WriteLine($"  Data: {importantDate.Label.Value} – {importantDate.Date}");
                return Unit.Instance;
            },
            () => Unit.Instance);

        return Unit.Instance;
    },
    e => { Console.WriteLine(e); return Unit.Instance; });
// =====================================================================
// Stan bazy
// =====================================================================
Console.WriteLine("--- Kategorie w bazie ---");
foreach (var category in categoryQueries.GetCategories())
{
    Console.WriteLine(category.Name);
}
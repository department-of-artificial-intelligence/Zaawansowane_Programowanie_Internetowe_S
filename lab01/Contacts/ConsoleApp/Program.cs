using Application.Domain;
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
var categoryRepository = new CategoryRepository(context);
var contactRepository = new ContactRepository(context);
var unitOfWork = new UnitOfWork(context);
var categoryUniquenessChecker = new CategoryUniquenessChecker(
    categoryRepository);
var categoryServices = new CategoryServices(
    categoryRepository, contactRepository, unitOfWork, categoryUniquenessChecker);
var contactServices = new ContactServices(
    contactRepository, categoryRepository, unitOfWork);

var addContactDto = new AddContactDto("Jan", "Kowalski", Sex.Male, 30);
var addContactResult = contactServices.AddContact(addContactDto);

Print("Dodanie kontaktu", addContactResult.Map(v =>
    $"{v.FirstName} {v.LastName} (id = {v.Id})"));

// Kategoria "Praca" może już istnieć po wcześniejszym uruchomieniu aplikacji.
var categoryId = categoryRepository.GetByName("Praca").Match(
    category => Result.Success(category.Id),
    () => categoryServices.AddCategory(new AddCategoryDto("Praca")).Map(c => c.Id));

var addEmailResult = addContactResult.Bind(contact =>
    categoryId.Bind(id =>
        contactServices.AddEmail(new AddEmailDto(
            contact.Id, "jan.kowalski@example.com", id))));
Print("Dodanie e-maila", addEmailResult);

// Zadanie 1 i 2 - walidacja obiektów wartości
Print("Kontakt z błędnymi danymi",
    contactServices.AddContact(new AddContactDto("", "K0walski", Sex.Male, 30)));
Print("Błędny adres e-mail", addContactResult.Bind(contact =>
    categoryId.Bind(id =>
        contactServices.AddEmail(new AddEmailDto(contact.Id, "jan.kowalski@", id)))));

// Zadanie 3 - zmiana nazwy kategorii
var tempCategory = categoryServices.AddCategory(
    new AddCategoryDto($"Tymczasowa {Guid.NewGuid():N}"));
Print("Zmiana nazwy kategorii na już istniejącą", tempCategory.Bind(c =>
    categoryServices.RenameCategory(c.Id, "Praca")));
Print("Zmiana nazwy kategorii", tempCategory.Bind(c =>
    categoryServices.RenameCategory(c.Id, $"Dom {Guid.NewGuid():N}")));

// Zadanie 4 - nie można usunąć kategorii, która jest używana
Print("Usunięcie używanej kategorii", categoryId.Bind(categoryServices.RemoveCategory));
Print("Usunięcie nieużywanej kategorii", tempCategory.Bind(c =>
    categoryServices.RemoveCategory(c.Id)));

// Zadanie 5 - edycja kontaktu
Print("Edycja kontaktu", addContactResult.Bind(contact =>
    contactServices.UpdateContact(new UpdateContactDto(
        contact.Id, "Janusz", "Kowalski-Nowak", Sex.Male, 31))));

// Zadanie 7 - numery telefonów
Print("Dodanie telefonu", addContactResult.Bind(contact =>
    contactServices.AddPhone(contact.Id, "+48 123-456-789")));
Print("Dodanie tego samego telefonu", addContactResult.Bind(contact =>
    contactServices.AddPhone(contact.Id, "+48123456789")));

// Zadanie 8 - ważne daty
Print("Dodanie daty urodzin", addContactResult.Bind(contact =>
    contactServices.AddImportantDate(contact.Id, new DateOnly(1994, 5, 12), "Urodziny")));
Print("Dodanie drugiej daty urodzin", addContactResult.Bind(contact =>
    contactServices.AddImportantDate(contact.Id, new DateOnly(1994, 5, 13), "urodziny")));

// Zadania 6, 7, 8 - usuwanie elementów kontaktu
Print("Usunięcie e-maila", addContactResult.Bind(contact =>
    contactServices.RemoveEmail(contact.Id, "jan.kowalski@example.com")));
Print("Usunięcie nieistniejącego e-maila", addContactResult.Bind(contact =>
    contactServices.RemoveEmail(contact.Id, "brak@example.com")));
Print("Usunięcie telefonu", addContactResult.Bind(contact =>
    contactServices.RemovePhone(contact.Id, "+48123456789")));
Print("Usunięcie daty", addContactResult.Bind(contact =>
    contactServices.RemoveImportantDate(contact.Id, "Urodziny")));

// Zadanie 5 - usunięcie kontaktu
Print("Usunięcie kontaktu", addContactResult.Bind(contact =>
    contactServices.RemoveContact(contact.Id)));

Console.WriteLine("\nKategorie:");
foreach(var category in context.Categories)
{
    Console.WriteLine($"  {category.Name}");
}

static void Print<T>(string operation, Result<T> result) =>
    Console.WriteLine(result.Match(
        _ => $"[OK]    {operation}",
        error => $"[BŁĄD]  {operation}: {error}"));

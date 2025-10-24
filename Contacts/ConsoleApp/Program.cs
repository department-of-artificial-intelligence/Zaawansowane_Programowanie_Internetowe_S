using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Contacts.Application.Domain; // Potrzebne dla Sex
using Contacts.Application.Repositories; // Potrzebne dla interfejsów
using Contacts.Application.Services; // Potrzebne dla serwisów
using Contacts.Application.UseCases.DTOs; // Potrzebne dla DTOs
using Contacts.Data; // Potrzebne dla AppDbContext i repozytoriów

// --- 1. Konfiguracja (bez zmian) ---
var builder = new ConfigurationBuilder();
builder.SetBasePath(AppContext.BaseDirectory)
    .AddJsonFile(
        "appsettings.json",
        optional: false,
        reloadOnChange: true);

IConfiguration config = builder.Build();
var dbOptionsBuilder = new DbContextOptionsBuilder<AppDbContext>(); // Użyj generycznej wersji
dbOptionsBuilder.UseSqlite(config.GetConnectionString("default"));

// --- 2. Stworzenie kontekstu i migracja ---
using var context = new AppDbContext(dbOptionsBuilder.Options);
context.Database.Migrate(); // Uruchamia migracje

Console.WriteLine("Aplikacja konsolowa uruchomiona...");

// --- 3. Inicjalizacja wszystkich repozytoriów i serwisów ---
// (To jest część, której brakowało i która powodowała błędy)
var unitOfWork = new UnitOfWork(context);
var categoryRepository = new CategoryRepository(context);
var categoryServices = new CategoryServices(categoryRepository, unitOfWork);
var contactRepository = new ContactRepository(context);

// ContactServices potrzebuje repozytorium kontaktów, kategorii (do walidacji) i unit of work
var contactServices = new ContactServices(contactRepository, categoryRepository, unitOfWork);


// --- 4. Nowy kod testowy dla dodawania kontaktu ---
// Używamy try...catch, aby łapać błędy walidacji
try
{
    // Krok A: Upewnij się, że kategoria "Praca" istnieje
    var categoryName = "Praca";
    var workCategory = categoryRepository.GetCategoryByName(categoryName);
    if (workCategory == null)
    {
        Console.WriteLine($"Tworzenie kategorii '{categoryName}'...");
        categoryServices.AddCategory(new AddCategoryDTO { Name = categoryName });
        workCategory = categoryRepository.GetCategoryByName(categoryName);
        Console.WriteLine("Kategoria utworzona.");
    }

    // Krok B: Stwórz DTO dla nowego kontaktu
    Console.WriteLine("Tworzenie nowego kontaktu 'Adam Nowak'...");
    var newContact = new AddContactDTO
    {
        FirstName = "Adam",
        LastName = "Nowak",
        Age = 25,
        Sex = Sex.Male,
        Emails = new List<AddContactEmailDTO>
        {
            new AddContactEmailDTO
            {
                Address = "adam.nowak@example.com",
                CategoryId = workCategory!.Id // Używamy ID kategorii "Praca"
            }
        }
    };

    // Krok C: Wywołaj serwis (używamy 'await', bo metoda jest asynchroniczna)
    await contactServices.AddContactAsync(newContact);

    Console.WriteLine("Sukces! Poprawnie dodano nowy kontakt.");
}
catch (Exception ex)
{
    Console.ForegroundColor = ConsoleColor.Red;
    Console.WriteLine($"\nWystąpił błąd podczas testowania aplikacji:");
    Console.WriteLine(ex.Message);
    Console.ResetColor();
}
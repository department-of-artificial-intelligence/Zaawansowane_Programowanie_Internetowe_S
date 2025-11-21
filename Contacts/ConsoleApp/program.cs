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
var dbOptionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
dbOptionsBuilder.UseSqlite(config.GetConnectionString("default"));

// --- 2. Stworzenie kontekstu i migracja ---
using var context = new AppDbContext(dbOptionsBuilder.Options);

Console.WriteLine("Stosowanie migracji do bazy danych...");
context.Database.Migrate(); // Uruchamia migracje
Console.WriteLine("Migracje zakończone.");

Console.WriteLine("Aplikacja konsolowa uruchomiona...");

// --- 3. Inicjalizacja wszystkich repozytoriów i serwisów ---
var unitOfWork = new UnitOfWork(context);
var categoryRepository = new CategoryRepository(context);
var categoryServices = new CategoryServices(categoryRepository, unitOfWork);
var contactRepository = new ContactRepository(context);

// Serwis kontaktów potrzebuje repozytoriów kontaktu, kategorii (do walidacji) i UoW
var contactServices = new ContactServices(contactRepository, categoryRepository, unitOfWork);

// --- 4. GŁÓWNY BLOK TESTOWY ---
try
{
    // --- TEST 1: Przygotowanie kategorii ---
    Console.WriteLine("\n--- TEST KATEGORII ---");

    // Wyszukaj lub stwórz kategorię "Praca"
    var pracaCategory = categoryRepository.GetCategoryByName("Praca");
    if (pracaCategory == null)
    {
        Console.WriteLine("Tworzenie kategorii 'Praca'...");
        categoryServices.AddCategory(new AddCategoryDTO { Name = "Praca" });
        pracaCategory = categoryRepository.GetCategoryByName("Praca");
    }
    Console.WriteLine($"Kategoria 'Praca' ma ID: {pracaCategory!.Id}");

    // Wyszukaj lub stwórz kategorię "Dom"
    var domCategory = categoryRepository.GetCategoryByName("Dom");
    if (domCategory == null)
    {
        Console.WriteLine("Tworzenie kategorii 'Dom'...");
        categoryServices.AddCategory(new AddCategoryDTO { Name = "Dom" });
        domCategory = categoryRepository.GetCategoryByName("Dom");
    }
    Console.WriteLine($"Kategoria 'Dom' ma ID: {domCategory!.Id}");


    // --- TEST 2: Dodawanie kontaktu (Zadanie 5) ---
    Console.WriteLine("\n--- TEST DODAWANIA KONTAKTU ---");
    // Ten test jest uproszczony - zakłada, że kontakt nie istnieje
    // W prawdziwej aplikacji sprawdzilibyśmy to
    
    Console.WriteLine("Tworzenie kontaktu 'Adam Nowak'...");
    var newContact = new AddContactDTO
    {
        FirstName = "Adam",
        LastName = "Nowak",
        Age = 30,
        Sex = Sex.Male,
        Emails = new List<AddContactEmailDTO>
        {
            new AddContactEmailDTO
            {
                Address = "adam.nowak@praca.com",
                CategoryId = pracaCategory.Id // Używamy ID kategorii "Praca"
            }
        }
    };
    await contactServices.AddContactAsync(newContact);
    Console.WriteLine("Dodano kontakt 'Adam Nowak'.");


    // --- TEST 3: Dodawanie Emaila (Zadanie 6) ---
    Console.WriteLine("\n--- TEST DODAWANIA EMAILA ---");
    Console.WriteLine("Dodawanie emaila 'adam@dom.pl' do kontaktu 1...");
    var newEmailDto = new AddEmailToContactDTO
    {
        ContactId = 1, // Zakładamy, że Adam Nowak dostał ID=1
        Address = "adam@dom.pl",
        CategoryId = domCategory.Id // Używamy ID kategorii "Dom"
    };
    await contactServices.AddEmailToContactAsync(newEmailDto);
    Console.WriteLine("Dodano email.");


    // --- TEST 4: Dodawanie Telefonu (Zadanie 7) ---
    Console.WriteLine("\n--- TEST DODAWANIA TELEFONU ---");
    Console.WriteLine("Dodawanie telefonu '555666777' do kontaktu 1...");
    var newPhoneDto = new AddPhoneToContactDTO
    {
        ContactId = 1, // Zakładamy, że Adam Nowak dostał ID=1
        Number = "555666777",
        CategoryId = pracaCategory.Id // Używamy ID kategorii "Praca"
    };
    await contactServices.AddPhoneToContactAsync(newPhoneDto);
    Console.WriteLine("Dodano telefon.");


    Console.ForegroundColor = ConsoleColor.Green;
    Console.WriteLine("\nSukces! Wszystkie testy 'Add' zakończone poprawnie.");
    Console.ResetColor();

    // Celowo nie testujemy usuwania, aby uniknąć problemu z zgadywaniem ID
        Console.WriteLine("\n--- TEST DODAWANIA WAŻNEJ DATY ---");
    Console.WriteLine("Dodawanie daty 'Urodziny' do kontaktu 1...");

    var newDateDto = new AddImportantDateToContactDTO
    {
        ContactId = 1, // Zakładamy, że Adam Nowak ma ID=1
        Date = new DateTime(1994, 5, 20), // Przykładowa data urodzin
        Description = "Urodziny"
    };
    await contactServices.AddImportantDateToContactAsync(newDateDto);
    Console.WriteLine("Dodano ważną datę.");

    // Aby przetestować usuwanie, musiałbyś znać ID daty (np. 1)
    // Console.WriteLine("Usuwanie daty o ID=1...");
    // var removeDateDto = new RemoveImportantDateFromContactDTO { ContactId = 1, ImportantDateId = 1 };
    // await contactServices.RemoveImportantDateFromContactAsync(removeDateDto);
    // Console.WriteLine("Usunięto datę.");
}
catch (Exception ex)
{
    // Jeśli którykolwiek test zawiedzie, zobaczysz błąd
    Console.ForegroundColor = ConsoleColor.Red;
    Console.WriteLine($"\nBłąd: {ex.Message}");
    Console.ResetColor();
}
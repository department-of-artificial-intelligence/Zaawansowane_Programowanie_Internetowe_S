using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Contacts.Application.Domain;
using Contacts.Application.Services;
using Contacts.Application.UseCases.DTOs;
using Contacts.Data;
using Application.UseCases.DTOs;
using Application.Domain;
// Wczytanie pliku konfiguracyjnego
var builder = new ConfigurationBuilder();
builder.SetBasePath(AppContext.BaseDirectory)
.AddJsonFile(
"appsettings.json",
optional: false,
reloadOnChange: true);
// Skonfigurowanie po!"czenia z baz" danych
IConfiguration config = builder.Build();
var dbOptionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
dbOptionsBuilder.UseSqlite(config.GetConnectionString("default"));
// Stworzenie kontekstu
using var context = new AppDbContext(dbOptionsBuilder.Options);
context.Database.Migrate();
context.Database.EnsureCreated();
// Uruchomienie aplikacji
var categoryRepository = new CategoryRepository(context);
var unitOfWork = new UnitOfWork(context);

// // --- ADD A CATEGORY (Adding two caegories to test removing) ---
// var initialCategory = new AddCategoryDTO { Name = "Default Category" };
// var secondCategory = new AddCategoryDTO { Name = "Second Category" };
// try
// {
//     // Call the AddCategory use case (Command)
//     categoryRepository.Add(initialCategory);
//     categoryRepository.Add(secondCategory);
//     unitOfWork.Save();
//     Console.WriteLine($"Added category: {initialCategory.Name}");
// }
// catch (Exception ex)
// {
//     Console.WriteLine($"Failed to add category (may already exist): {ex.Message}");
// }
// // -----------------------------------------------------------------
// var categoryServices = new CategoryServices(categoryRepository, unitOfWork);
// categoryServices.RemoveCategory(1);
// foreach (var category in context.Categories)
// {
//     Console.WriteLine(category.Name);
// }

// // --- TEST SCENARIO: MODIFY CATEGORY ---

// // 1. ADD INITIAL CATEGORY
// var categoryServices = new CategoryServices(categoryRepository, unitOfWork);
// const string initialName = "Old Category Name";
// var addDto = new AddCategoryDTO { Name = initialName };

// try
// {
//     // Komenda do dodania kategorii
//     categoryServices.AddCategory(addDto);
//     Console.WriteLine($"✅ Added category: '{initialName}'.");
// }
// catch (Exception ex)
// {
//     Console.WriteLine($"❌ Failed to add category: {ex.Message}");
//     return;
// }

// // 2. RETRIEVE CATEGORY ID (assuming ID 1 for the first insertion)
// var category = categoryRepository.GetCategoryByName(initialName);
// if (category == null)
// {
//     Console.WriteLine("❌ ERROR: Newly added category not found.");
//     return;
// }
// int categoryIdToUpdate = category.Id;
// Console.WriteLine($"🔍 Category found with ID: {categoryIdToUpdate}.");


// // 3. UPDATE CATEGORY NAME
// const string newName = "New Updated Category";
// var updateDto = new UpdateCategoryDTO { Id = categoryIdToUpdate, Name = newName };

// try
// {
//     // Komenda do modyfikacji kategorii
//     categoryServices.UpdateCategory(updateDto);
//     unitOfWork.Save(); // Ensure Save is called after UpdateCategory!
//     Console.WriteLine($"✅ Successfully updated category ID {categoryIdToUpdate} to '{newName}'.");
// }
// catch (Exception ex)
// {
//     Console.WriteLine($"❌ Failed to update category: {ex.Message}");
//     return;
// }


// // 4. DISPLAY FINAL RESULTS
// Console.WriteLine("\n--- Current categories after update: ---");
// foreach (var finalCategory in context.Categories)
// {
//     Console.WriteLine($"ID: {finalCategory.Id}, Name: {finalCategory.Name}");
// }
// Console.WriteLine("---------------------------------------");

// Uruchomienie aplikacji
var categoryServices = new CategoryServices(categoryRepository, unitOfWork);

// --- SCENARIUSZ TESTOWY USUWANIA KATEGORII (Z WALIDACJĄ UŻYCIA) ---

// 1. Upewnienie się, że baza jest czysta (Usuń wszystko dla pewności)
context.Contacts.RemoveRange(context.Contacts);
context.Categories.RemoveRange(context.Categories);
unitOfWork.Save();
Console.WriteLine("Baza danych wyczyszczona.");

// --- KROK 1: Przygotowanie danych ---

// Dodaj dwie kategorie: jedną do usunięcia, drugą do powiązania
var cat1Dto = new AddCategoryDTO { Name = "Category_Unused" };
var cat2Dto = new AddCategoryDTO { Name = "Category_Used" };
categoryServices.AddCategory(cat1Dto);
categoryServices.AddCategory(cat2Dto);

// Pobierz ID, zakładając, że ID są 1 i 2
var unusedCategory = categoryRepository.GetCategoryByName("Category_Unused");
var usedCategory = categoryRepository.GetCategoryByName("Category_Used");
int unusedId = unusedCategory.Id;
int usedId = usedCategory.Id;

Console.WriteLine($"\nDodano kategorie (ID {unusedId} i {usedId}).");

// Powiąż Kategorię Używaną (ID 2) z nowym kontaktem (wymaga istnienia Contact i Email)
// Załóżmy, że tworzysz nowy Contact i Email, używając obiektów z domeny:
// UWAGA: Ten blok wymaga, abyś miał już zaimplementowane i działające: 
//        FirstName, LastName, Age (Value Objects) oraz Contact i Email (Entities).
try
{
    // Przygotowanie danych dla Contact i Email (w uproszczeniu, zakładając istnienie konstruktorów)
    var testContact = new Contact(
        id: 0,
        firstName: new FirstName("Test"),
        lastName: new LastName("User"),
        sex: Sex.Male,
        emails: new List<Email>(),
        age: new Age(30)
    );

    // Dodanie kontaktu do kontekstu, aby uzyskać ID
    context.Contacts.Add(testContact);
    unitOfWork.Save(); // Zapisanie Contact, aby uzyskać jego ID

    // Tworzenie nowego Emaila powiązanego z używaną kategorią (ID 2)
    var testEmail = new Email(
        id: 0,
        contact: testContact,
        category: usedCategory, // Używa kategorii ID 2
        email: new EmailAddress("test@used.pl")
    );

    testContact.AddEmail(testEmail);
    unitOfWork.Save();
    Console.WriteLine($"Powiązano adres email z kategorią ID {usedId}.");
}
catch (Exception ex)
{
    Console.WriteLine($"❌ Błąd przy tworzeniu Contact/Email: {ex.Message}. Upewnij się, że masz zaimplementowane Value Objects!");
    return;
}

// --- KROK 2: Test 1: Nieudane usunięcie (Kategoria Używana) ---
Console.WriteLine("\n--- TEST 1: USUWANIE UŻYWANEJ KATEGORII ---");
try
{
    categoryServices.RemoveCategory(usedId);
    Console.WriteLine("❌ BŁĄD TESTU: Usunięcie powiązanej kategorii ZAKOŃCZYŁO SIĘ SUKCESEM. Oczekiwano wyjątku.");
}
catch (ServiceException ex)
{
    Console.WriteLine($"✅ TEST 1 SUKCES: Oczekiwany błąd biznesowy: {ex.Message}");
}
catch (Exception ex)
{
    Console.WriteLine($"❌ BŁĄD TESTU: Został rzucony nieoczekiwany wyjątek: {ex.Message}");
}

// --- KROK 3: Test 2: Udane usunięcie (Kategoria Nieużywana) ---
Console.WriteLine("\n--- TEST 2: USUWANIE NIEUŻYWANEJ KATEGORII ---");
try
{
    categoryServices.RemoveCategory(unusedId);
    Console.WriteLine($"✅ TEST 2 SUKCES: Usunięto kategorię ID {unusedId}.");
}
catch (Exception ex)
{
    Console.WriteLine($"❌ BŁĄD TESTU: Wystąpił błąd przy usuwaniu nieużywanej kategorii: {ex.Message}");
}

// --- KROK 4: Weryfikacja końcowa ---
Console.WriteLine("\n--- STAN KOŃCOWY KATEGORII ---");
foreach (var cat in context.Categories)
{
    Console.WriteLine($"ID: {cat.Id}, Name: {cat.Name}");
}
Console.WriteLine("-----------------------------");

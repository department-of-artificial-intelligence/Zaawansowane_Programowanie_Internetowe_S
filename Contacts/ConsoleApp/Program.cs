using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using Contacts.Application.Domain;
using Contacts.Application.Services;
using Contacts.Application.UseCases.DTOs;
using Contacts.Data;

// Wczytanie pliku konfiguracyjnego
var builder = new ConfigurationBuilder();
builder.SetBasePath(AppContext.BaseDirectory)
    .AddJsonFile(
        "appsettings.json",
        optional: false,
        reloadOnChange: true
    );

// Skonfigurowanie połączenia z bazą danych 
IConfiguration config = builder.Build();
var dbOptionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
dbOptionsBuilder.UseSqlite(config.GetConnectionString("default"));

// Stworzenie kontekstu 
using var context = new AppDbContext(dbOptionsBuilder.Options);
context.Database.Migrate();

// Utworzenie repozytorium i serwisów
var categoryRepository = new CategoryRepository(context);
var unitOfWork = new UnitOfWork(context);
var categoryServices = new CategoryServices(categoryRepository, unitOfWork);
var contactRepository = new ContactRepository(context);
var contactServices = new ContactServices(contactRepository, unitOfWork);
var emailRepository = new EmailRepository(context);
var emailServices = new EmailServices(emailRepository, contactRepository, categoryRepository, unitOfWork);
var phoneRepository = new PhoneRepository(context);
var phoneServices = new PhoneServices(phoneRepository, contactRepository, categoryRepository, unitOfWork);
var importantDateRepository = new ImportantDateRepository(context);
var importantDateServices = new ImportantDateServices(importantDateRepository, contactRepository, categoryRepository, unitOfWork);

// Dodanie przykładowej kategorii
try
{
    var addCategoryDTO = new AddCategoryDTO { Name = "Przykładowa kategoria" };
    var result = categoryServices.AddCategory(addCategoryDTO);
    Console.WriteLine($"Dodano kategorię: {result.Name} (ID: {result.Id})");
}
catch (ServiceException ex)
{
    Console.WriteLine($"Błąd przy dodawaniu kategorii: {ex.Message}");
}

// Wybór kategorii do demonstracji zasad usuwania
var categoryToUse = context.Categories.FirstOrDefault();
if (categoryToUse != null)
{
    // Utworzenie kontaktu i emaila przypisanego do wybranej kategorii
    var contact = new Contact(0, "Jan", "Kowalski", Sex.Male, new List<Email>(), new Age(30));
    context.Contacts.Add(contact);
    context.SaveChanges();

    var email = new Email(0, contact, categoryToUse, new EmailAddress("jan.kowalski@example.com"));
    context.Emails.Add(email);
    context.SaveChanges();

    Console.WriteLine($"\nDodano email dla kontaktu {contact.Id} w kategorii {categoryToUse.Id}");
}

// Wyświetlenie wszystkich kategorii
Console.WriteLine("\nWszystkie kategorie:");
foreach (var category in context.Categories)
{
    Console.WriteLine($"ID: {category.Id}, Nazwa: {category.Name}");
}

// Próba usunięcia kategorii (z zachowaniem reguły: nie usuwać gdy są przypisane emaile)
try
{
    var idToDelete = categoryToUse?.Id ?? 1; // jeśli brak kategorii, spróbuj ID=1
    categoryServices.RemoveCategory(idToDelete);
    Console.WriteLine($"Usunięto kategorię o ID = {idToDelete}");
}
catch (ServiceException ex)
{
    Console.WriteLine($"Błąd przy usuwaniu kategorii: {ex.Message}");
}

// Wyświetlenie kategorii po próbie usunięcia
Console.WriteLine("\nKategorie po usunięciu:");
foreach (var category in context.Categories)
{
    Console.WriteLine($"ID: {category.Id}, Nazwa: {category.Name}");
}

// --- DEMO: Dodawanie, edycja i usuwanie kontaktu ---
Console.WriteLine("\n--- Kontakty: dodanie, edycja, usunięcie ---");

// Dodanie nowego kontaktu
try
{
    var addContactResult = contactServices.AddContact(new AddContactDTO
    {
        Firstname = "Adam",
        LastName = "Nowak",
        Sex = Sex.Male,
        Age = 25
    });
    Console.WriteLine($"Dodano kontakt: ID={addContactResult.Id}, {addContactResult.Firstname} {addContactResult.LastName}, Wiek={addContactResult.Age}");

    // Edycja kontaktu
    contactServices.UpdateContact(new UpdateContactDTO
    {
        Id = addContactResult.Id,
        Firstname = addContactResult.Firstname,
        LastName = "Nowakowski",
        Sex = addContactResult.Sex,
        Age = 26
    });
    var updated = context.Contacts.Find(addContactResult.Id)!;
    Console.WriteLine($"Zaktualizowano kontakt: ID={updated.Id}, {updated.Firstname} {updated.LastName}, Wiek={updated.Age.Value}");

    // Usunięcie kontaktu
    contactServices.RemoveContact(addContactResult.Id);
    Console.WriteLine($"Usunięto kontakt o ID={addContactResult.Id}");
}
catch (ServiceException ex)
{
    Console.WriteLine($"Błąd operacji na kontakcie: {ex.Message}");
}

// Lista kontaktów po operacjach
Console.WriteLine("\nKontakty po operacjach:");
foreach (var c in context.Contacts)
{
    Console.WriteLine($"ID={c.Id}, {c.Firstname} {c.LastName}, Wiek={c.Age.Value}");
}
// --- DEMO: Dołączanie i usuwanie adresu email z kontaktu ---
Console.WriteLine("\n--- Email: dołączanie i usuwanie z kontaktu ---");

try
{
    var targetContactId = context.Contacts.First().Id;
    var targetCategoryId = context.Categories.First().Id;

    var addEmailResult = emailServices.AddEmail(new AddEmailDTO
    {
        ContactId = targetContactId,
        CategoryId = targetCategoryId,
        Address = "adam.nowak@example.com"
    });
    Console.WriteLine($"Dodano email ID={addEmailResult.Id} do kontaktu {addEmailResult.ContactId}: {addEmailResult.Address}");

    Console.WriteLine("Aktualne emaile kontaktu:");
    foreach (var e in context.Emails.Where(e => e.ContactId == targetContactId))
    {
        Console.WriteLine($"EmailID={e.Id}, {e.Address}, KategoriaID={e.CategoryId}");
    }

    emailServices.RemoveEmail(addEmailResult.Id);
    Console.WriteLine($"Usunięto email ID={addEmailResult.Id} z kontaktu {targetContactId}");

    Console.WriteLine("Emaile kontaktu po usunięciu:");
    foreach (var e in context.Emails.Where(e => e.ContactId == targetContactId))
    {
        Console.WriteLine($"EmailID={e.Id}, {e.Address}, KategoriaID={e.CategoryId}");
    }
}
catch (ServiceException ex)
{
    Console.WriteLine($"Błąd operacji na emailu: {ex.Message}");
}

// --- DEMO: Dołączanie i usuwanie numeru telefonu z kontaktu ---
Console.WriteLine("\n--- Telefon: dołączanie i usuwanie z kontaktu ---");

try
{
    var targetContactId = context.Contacts.First().Id;
    var targetCategoryId = context.Categories.First().Id;

    var addPhoneResult = phoneServices.AddPhone(new AddPhoneDTO
    {
        ContactId = targetContactId,
        CategoryId = targetCategoryId,
        Number = "+48123456789"
    });
    Console.WriteLine($"Dodano telefon ID={addPhoneResult.Id} do kontaktu {addPhoneResult.ContactId}: {addPhoneResult.Number}");

    Console.WriteLine("Aktualne telefony kontaktu:");
    foreach (var p in context.Phones.Where(p => p.ContactId == targetContactId))
    {
        Console.WriteLine($"PhoneID={p.Id}, {p.Number}, KategoriaID={p.CategoryId}");
    }

    phoneServices.RemovePhone(addPhoneResult.Id);
    Console.WriteLine($"Usunięto telefon ID={addPhoneResult.Id} z kontaktu {targetContactId}");

    Console.WriteLine("Telefony kontaktu po usunięciu:");
    foreach (var p in context.Phones.Where(p => p.ContactId == targetContactId))
    {
        Console.WriteLine($"PhoneID={p.Id}, {p.Number}, KategoriaID={p.CategoryId}");
    }
}
catch (ServiceException ex)
{
    Console.WriteLine($"Błąd operacji na telefonie: {ex.Message}");
}

// --- DEMO: Dołączanie i usuwanie ważnej daty z kontaktu ---
Console.WriteLine("\n--- Ważne daty: dołączanie i usuwanie z kontaktu ---");

try
{
    var targetContactId = context.Contacts.First().Id;
    var targetCategoryId = context.Categories.First().Id;

    var addDateResult = importantDateServices.AddImportantDate(new AddImportantDateDTO
    {
        ContactId = targetContactId,
        CategoryId = targetCategoryId,
        Date = new DateOnly(2025, 1, 1),
        Description = "Urodziny"
    });
    Console.WriteLine($"Dodano ważną datę ID={addDateResult.ImportantDateId} do kontaktu {targetContactId}: {new DateOnly(2025,1,1):yyyy-MM-dd} - Urodziny");

    Console.WriteLine("Aktualne ważne daty kontaktu:");
    foreach (var d in context.ImportantDates.Where(d => d.ContactId == targetContactId))
    {
        Console.WriteLine($"DateID={d.Id}, {d.Info.Date:yyyy-MM-dd} - {d.Info.Description}, KategoriaID={d.CategoryId}");
    }

    importantDateServices.RemoveImportantDate(addDateResult.ImportantDateId);
    Console.WriteLine($"Usunięto ważną datę ID={addDateResult.ImportantDateId} z kontaktu {targetContactId}");

    Console.WriteLine("Ważne daty kontaktu po usunięciu:");
    foreach (var d in context.ImportantDates.Where(d => d.ContactId == targetContactId))
    {
        Console.WriteLine($"DateID={d.Id}, {d.Info.Date:yyyy-MM-dd} - {d.Info.Description}, KategoriaID={d.CategoryId}");
    }
}
catch (ServiceException ex)
{
    Console.WriteLine($"Błąd operacji na ważnej dacie: {ex.Message}");
}
// --- KONIEC DEMO ---

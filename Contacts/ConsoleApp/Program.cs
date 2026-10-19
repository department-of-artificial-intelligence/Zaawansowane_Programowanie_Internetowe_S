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

IConfiguration config = builder.Build();
var dbOptionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
dbOptionsBuilder.UseSqlite(config.GetConnectionString("default"));

using var context = new AppDbContext(dbOptionsBuilder.Options);
context.Database.Migrate();

var categoryRepository = new CategoryRepository(context);
var unitOfWork = new UnitOfWork(context);
var categoryUniquenessChecker = new CategoryUniquenessChecker(categoryRepository);
var categoryServices = new CategoryServices(categoryRepository, unitOfWork, categoryUniquenessChecker);

var contactRepository = new ContactRepository(context);
var contactServices = new ContactServices(contactRepository, categoryRepository, unitOfWork);

// 1. Dodanie kontaktu
var addContactDto = new AddContactDto("Adam", "Nowak", Sex.Male, 28);
var addContactResult = contactServices.AddContact(addContactDto);

addContactResult.Match(
    contact =>
    {
        Console.WriteLine($"[OK] Dodano kontakt: {contact.FirstName} {contact.LastName} (ID: {contact.Id})");

        // 2. Dodanie telefonu (Zadanie 7)
        var category = categoryRepository.GetAll().FirstOrDefault();
        if (category != null)
        {
            var addPhoneResult = contactServices.AddPhone(new AddPhoneDto(contact.Id, "+48123456789", category.Id));
            addPhoneResult.Match(
                _ => { Console.WriteLine("[OK] Dodano numer telefonu."); return Unit.Instance; },
                err => { Console.WriteLine($"[Błąd telefonu]: {err}"); return Unit.Instance; }
            );

            // 3. Dodanie waznej daty (Zadanie 8)
            var addDateResult = contactServices.AddImportantDate(new AddImportantDateDto(contact.Id, "Urodziny", new DateOnly(1996, 5, 20)));
            addDateResult.Match(
                _ => { Console.WriteLine("[OK] Dodano ważną datę."); return Unit.Instance; },
                err => { Console.WriteLine($"[Błąd daty]: {err}"); return Unit.Instance; }
            );
        }

        return Unit.Instance;
    },
    err =>
    {
        Console.WriteLine($"[Błąd kontaktu]: {err}");
        return Unit.Instance;
    }
);

Console.WriteLine("\n--- Lista kontaktów w bazie ---");
foreach (var c in contactRepository.GetAll())
{
    Console.WriteLine($"Kontakt: {c.FirstName.Value} {c.LastName.Value}, Emaili: {c.Emails.Count}, Telefonów: {c.Phones.Count}, Dat: {c.ImportantDates.Count}");
}
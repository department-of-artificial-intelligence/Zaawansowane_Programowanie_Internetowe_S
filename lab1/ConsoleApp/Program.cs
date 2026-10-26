
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

IConfiguration config = builder.Build();

var dbOptionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
dbOptionsBuilder.UseSqlite(config.GetConnectionString("default"));

using var context = new AppDbContext(dbOptionsBuilder.Options);
context.Database.Migrate();

var categoryRepository = new CategoryRepository(context);
var contactRepository = new ContactRepository(context);
var unitOfWork = new UnitOfWork(context);

var categoryUniquenessChecker =
    new CategoryUniquenessChecker(categoryRepository);

var categoryServices = new CategoryServices(
    categoryRepository,
    contactRepository,
    unitOfWork,
    categoryUniquenessChecker);

var contactServices = new ContactServices(
    contactRepository,
    categoryRepository,
    unitOfWork);

var categoryName = "EmailTest";
var category = context.Categories
    .FirstOrDefault(c => c.Name == categoryName);

if (category is null)
{
    var categoryResult = categoryServices.AddCategory(
        new AddCategoryDto(categoryName));

    categoryResult.Match(
        _ =>
        {
            Console.WriteLine("Utworzono kategorię EmailTest.");
            return Unit.Instance;
        },
        error =>
        {
            Console.WriteLine($"Błąd tworzenia kategorii: {error}");
            return Unit.Instance;
        });
}

category = context.Categories
    .FirstOrDefault(c => c.Name == categoryName);

if (category is null)
{
    Console.WriteLine("Nie udało się przygotować kategorii.");
    return;
}

var contactResult = contactServices.AddContact(
    new AddContactDto("Email", "Testowy", Sex.Male, 25));

var testContactId = Guid.Empty;

contactResult.Match(
    contact =>
    {
        testContactId = contact.Id;
        Console.WriteLine($"Utworzono kontakt testowy: {contact.Id}");
        return Unit.Instance;
    },
    error =>
    {
        Console.WriteLine($"Błąd tworzenia kontaktu: {error}");
        return Unit.Instance;
    });

if (testContactId == Guid.Empty)
{
    return;
}

const string testAddress = "email.testowy@example.com";

var addEmailResult = contactServices.AddEmail(
    new AddEmailDto(testContactId, testAddress, category.Id));

var emailAdded = false;

addEmailResult.Match(
    _ =>
    {
        emailAdded = true;
        Console.WriteLine("Dodano adres e-mail.");
        return Unit.Instance;
    },
    error =>
    {
        Console.WriteLine($"Błąd dodawania e-maila: {error}");
        return Unit.Instance;
    });

if (!emailAdded)
{
    contactServices.RemoveContact(testContactId);
    return;
}

var removeEmailResult =
    contactServices.RemoveEmail(testContactId, testAddress);

removeEmailResult.Match(
    _ =>
    {
        Console.WriteLine("Usunięto adres e-mail.");
        return Unit.Instance;
    },
    error =>
    {
        Console.WriteLine($"Błąd usuwania e-maila: {error}");
        return Unit.Instance;
    });

const string testPhoneNumber = "+48123456789";

var addPhoneResult = contactServices.AddPhone(
    new AddPhoneDto(testContactId, testPhoneNumber));

var phoneAdded = false;

addPhoneResult.Match(
    _ =>
    {
        phoneAdded = true;
        Console.WriteLine("Dodano numer telefonu.");
        return Unit.Instance;
    },
    error =>
    {
        Console.WriteLine($"Błąd dodawania telefonu: {error}");
        return Unit.Instance;
    });

if (!phoneAdded)
{
    contactServices.RemoveContact(testContactId);
    return;
}

var removePhoneResult =
    contactServices.RemovePhone(testContactId, testPhoneNumber);

removePhoneResult.Match(
    _ =>
    {
        Console.WriteLine("Usunięto numer telefonu.");
        return Unit.Instance;
    },
    error =>
    {
        Console.WriteLine($"Błąd usuwania telefonu: {error}");
        return Unit.Instance;
    });

var importantDate = new DateOnly(1999, 5, 12);
const string importantDateName = "Urodziny";

var addImportantDateResult = contactServices.AddImportantDate(
    new AddImportantDateDto(
        testContactId,
        importantDateName,
        importantDate));

var importantDateAdded = false;

addImportantDateResult.Match(
    _ =>
    {
        importantDateAdded = true;
        Console.WriteLine("Dodano ważną datę.");
        return Unit.Instance;
    },
    error =>
    {
        Console.WriteLine($"Błąd dodawania ważnej daty: {error}");
        return Unit.Instance;
    });

if (!importantDateAdded)
{
    contactServices.RemoveContact(testContactId);
    return;
}

var savedContact = contactRepository.GetById(testContactId);

savedContact.Match(
    contact =>
    {
        Console.WriteLine($"Kontakt nadal istnieje: {contact.Id}");
        Console.WriteLine($"Liczba adresów e-mail: {contact.Emails.Count}");
        Console.WriteLine($"Liczba numerów telefonu: {contact.Phones.Count}");
        Console.WriteLine($"Liczba ważnych dat: {contact.ImportantDates.Count}");

        foreach (var date in contact.ImportantDates)
        {
            Console.WriteLine($"{date.Name}: {date.Date:yyyy-MM-dd}");
        }

        return Unit.Instance;
    },
    () =>
    {
        Console.WriteLine("Nie znaleziono kontaktu testowego.");
        return Unit.Instance;
    });

var removeImportantDateResult =
    contactServices.RemoveImportantDate(
        testContactId,
        importantDateName,
        importantDate);

removeImportantDateResult.Match(
    _ =>
    {
        Console.WriteLine("Usunięto ważną datę.");
        return Unit.Instance;
    },
    error =>
    {
        Console.WriteLine($"Błąd usuwania ważnej daty: {error}");
        return Unit.Instance;
    });

contactServices.RemoveContact(testContactId);

Console.WriteLine("Usunięto kontakt testowy.");

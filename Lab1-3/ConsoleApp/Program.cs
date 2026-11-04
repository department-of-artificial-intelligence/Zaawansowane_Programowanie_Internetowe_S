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

addContactResult.Match(
    v => {
        Console.WriteLine(
            $"Dodano kontakt {v.FirstName} {v.LastName} (id = {v.Id})");
        return Unit.Instance;
    },
    e => { Console.WriteLine(e); return Unit.Instance; }
);

var addEmailResult = addContactResult.Bind(contact =>
    categoryServices.AddCategory(new AddCategoryDto("Praca"))
        .Bind(category =>
        {
            var addEmailDto = new AddEmailDto(
                contact.Id, "jan.kowalski@example.com", category.Id);
            return contactServices.AddEmail(addEmailDto);
        }));

addEmailResult.Match(
    _ => { Console.WriteLine("Dodano e-mail."); return Unit.Instance; },
    e => { Console.WriteLine(e); return Unit.Instance; }
);

foreach (var category in context.Categories)
{
    Console.WriteLine(category.Name);
}


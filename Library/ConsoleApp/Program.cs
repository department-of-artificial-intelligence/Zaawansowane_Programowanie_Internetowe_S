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
        Console.WriteLine($"Dodano kontakt {v.FirstName} {v.LastName} (id = {v.Id})");
        return Unit.Instance;
    },
    e => {Console.WriteLine(e); return Unit.Instance;}
);

var addEmailResult = addContactResult.Bind(contact =>
    categoryServices.AddCategory(new AddCategoryDto("Praca"))
        .Bind(category =>
        {
            var addEmailDto = new AddEmailDto(
                contact.Id, "jan.kowalski@example.com", category.Id);
            return contactServices.AddEmail(addEmailDto);
        })
);

addEmailResult.Match(
    _ => {Console.WriteLine("Dodano e-mail."); return Unit.Instance;},
    e => {Console.WriteLine(e); return Unit.Instance;}
);

foreach(var category in context.Categories)
{
    Console.WriteLine(category.Name);
}

var categoryToUpdate = context.Categories.FirstOrDefault(c=>c.Name == "Praca");
if(categoryToUpdate != null){
    var updateDto = new UpdateCategoryDto(categoryToUpdate.Id, "Praca Zdalna");
    var updateResult = categoryServices.UpdateCategory(updateDto);

    updateResult.Match(
        _ => {
            Console.WriteLine($"Zaktualizowano kategorię na 'Praca Zdalna'.");
            return Unit.Instance;
        },
        e => {Console.WriteLine(e); return Unit.Instance;}
    );
}

Console.WriteLine("Aktualna Lista: ");
foreach(var category in context.Categories)
{
    Console.WriteLine(category.Name);
}
Console.WriteLine("Testowanie usuwania kategorii z email");
var categoryToDelete = context.Categories.FirstOrDefault(c=>c.Name == "Praca Zdalna");

if(categoryToDelete != null){
    var deleteResult = categoryServices.RemoveCategory(categoryToDelete.Id);
    deleteResult.Match(
        _ => {Console.WriteLine("Kategoria została usunięta."); return Unit.Instance;},
        e => {Console.WriteLine(e); return Unit.Instance;}
    );
}
Console.WriteLine("Testowanie usuwania pustej kategorii");
var newCategoryResult = categoryServices.AddCategory(new AddCategoryDto("Tymczasowa"));
newCategoryResult.Match(
    _ => {Console.WriteLine("Dodano kategorie."); return Unit.Instance;},
    e => {Console.WriteLine(e); return Unit.Instance;}
);

var categoryNewToDelete = context.Categories.FirstOrDefault(c=>c.Name == "Tymczasowa");
if(categoryNewToDelete != null){
    var deleteResult = categoryServices.RemoveCategory(categoryNewToDelete.Id);
    deleteResult.Match(
        _ => {Console.WriteLine("Kategoria została usunięta."); return Unit.Instance;},
        e => {Console.WriteLine(e); return Unit.Instance;}
    );
}
Console.WriteLine("Edycja i usuwanie kontaktu");

var contactToUpdate = context.Contacts.FirstOrDefault(c => c.FirstName.Value == "Jan");
if (contactToUpdate != null)
{
    var updateDto = new UpdateContactDto(
        contactToUpdate.Id,
        "Janusz",
        "Nowak",
        Sex.Male,
        31
    );

    var updateResult = contactServices.UpdateContact(updateDto);

    updateResult.Match(
        _ => {Console.WriteLine("Zaktualizowano dane kontaktu."); return Unit.Instance;},
        e => {Console.WriteLine(e); return Unit.Instance;}
    );

    var removeResult = contactServices.RemoveContact(contactToUpdate.Id);

    removeResult.Match(
         _ => {Console.WriteLine("Kontakt został usunięty."); return Unit.Instance;},
        e => {Console.WriteLine(e); return Unit.Instance;}
    );
}
else
{
    Console.WriteLine("Nie znaleziono kontaktu do przetestowania.");
}
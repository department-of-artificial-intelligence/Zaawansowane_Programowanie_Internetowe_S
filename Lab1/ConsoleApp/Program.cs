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
var contactRepository = new ContactRepository(context);
var categoryRepository = new CategoryRepository(context);
var unitOfWork = new UnitOfWork(context);
var categoryServices = new CategoryServices(
    categoryRepository, unitOfWork,
    new CategoryUniquenessChecker(categoryRepository),
    new CategoryRemovalChecker(contactRepository));
var contactServices = new ContactServices(
    contactRepository, categoryRepository, unitOfWork);

void Print<T>(string step, Result<T> result) =>
    result.Match(
        _ => { Console.WriteLine($"OK   {step}"); return Unit.Instance; },
        e => { Console.WriteLine($"BŁĄD {step}: {e}"); return Unit.Instance; });

var suffix = DateTime.Now.Ticks;   // unikalne nazwy kategorii przy kolejnych uruchomieniach

Print("zad. 1: puste imię (oczekiwany błąd)",
    contactServices.AddContact(new AddContactDto(" ", "Nowak", Sex.Male, 30)));
var contact = contactServices.AddContact(new AddContactDto("Jan", "Kowalski", Sex.Male, 30));
Print("zad. 1: dodanie kontaktu", contact);
var contactId = contact.Match(c => c.Id, _ => Guid.Empty);

var category = categoryServices.AddCategory(new AddCategoryDto($"Praca {suffix}"));
var categoryId = category.Match(c => c.Id, _ => Guid.Empty);

Print("zad. 2: zły e-mail (oczekiwany błąd)",
    contactServices.AddEmail(new AddEmailDto(contactId, "jan@", categoryId)));
Print("zad. 2: dobry e-mail",
    contactServices.AddEmail(new AddEmailDto(contactId, "Jan.Kowalski@Example.com", categoryId)));
Print("zad. 3: zmiana nazwy kategorii",
    categoryServices.RenameCategory(new RenameCategoryDto(categoryId, $"Służbowe {suffix}")));
Print("zad. 4: usunięcie używanej kategorii (oczekiwany błąd)",
    categoryServices.RemoveCategory(categoryId));
Print("zad. 6: usunięcie e-maila",
    contactServices.RemoveEmail(new RemoveEmailDto(contactId, "jan.kowalski@example.com")));
Print("zad. 4: usunięcie wolnej kategorii",
    categoryServices.RemoveCategory(categoryId));
Print("zad. 5: edycja kontaktu",
    contactServices.UpdateContact(new UpdateContactDto(contactId, "Janusz", "Kowalski", Sex.Male, 31)));
Print("zad. 7: dodanie telefonu",
    contactServices.AddPhone(new AddPhoneDto(contactId, "+48 600-100-200", PhoneType.Mobile)));
Print("zad. 7: ten sam numer inaczej zapisany (oczekiwany błąd)",
    contactServices.AddPhone(new AddPhoneDto(contactId, "+48600100200", PhoneType.Work)));
Print("zad. 7: usunięcie telefonu",
    contactServices.RemovePhone(new RemovePhoneDto(contactId, "+48600100200")));
var date = contactServices.AddImportantDate(
    new AddImportantDateDto(contactId, "Urodziny", new DateOnly(1995, 5, 12)));
Print("zad. 8: dodanie daty", date);
var dateId = date.Match(id => id, _ => Guid.Empty);
Print("zad. 8: zmiana daty", contactServices.ChangeImportantDate(
    new ChangeImportantDateDto(contactId, dateId, "Urodziny", new DateOnly(1995, 5, 13))));
Print("zad. 8: usunięcie daty", contactServices.RemoveImportantDate(
    new RemoveImportantDateDto(contactId, dateId)));
Print("zad. 5: usunięcie kontaktu", contactServices.RemoveContact(contactId));

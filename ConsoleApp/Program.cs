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

// czyszcze baze aby byl swiezy stan testow
context.Database.EnsureDeleted();
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

/*
addContactResult.Match(
    v => {
        Console.WriteLine(
            $"Dodano kontakt {v.FirstName} {v.LastName} (id = {v.Id})");
        return Unit.Instance;
    },
    e => {Console.WriteLine(e); return Unit.Instance;}
    );
*/
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


Console.WriteLine("Test zad 1");
// zle imie
var invalidFirstName = FirstName.Create("");
invalidFirstName.Match(
    succ => { Console.WriteLine($"Sukces: {succ.Value}"); return Unit.Instance; },
    fail => { Console.WriteLine($" Blad: {fail}"); return Unit.Instance; }
);

// poprawne imie i nazwisko
var validFirstName = FirstName.Create("Jan");
var validLastName = LastName.Create("Kowalski");

Console.WriteLine($" Poprawne imie: {validFirstName.Match(s => s.Value, _ => "")}");
Console.WriteLine($" Poprawne nazwisko: {validLastName.Match(s => s.Value, _ => "")}");


Console.WriteLine("Test zad 2");

// zly mail

var invalidEmail = EmailAddress.Create("zlyadres");
invalidEmail.Match(
    succ => { Console.WriteLine($"Sukces: {succ.Value}"); return Unit.Instance; },
    fail => { Console.WriteLine($"Blad: {fail}"); return Unit.Instance; }
);

// dobry mail

var validEmail = EmailAddress.Create("123@gmail.com");
validEmail.Match(
    succ => { Console.WriteLine($"Sukces: {succ.Value}"); return Unit.Instance; },
    fail => { Console.WriteLine($"Blad: {fail}"); ; return Unit.Instance; }
);


Console.WriteLine("Test zad 3");

var AddCategoryResult = categoryServices.AddCategory(new AddCategoryDto("JakasKategoria2"));
AddCategoryResult.Match(
    result =>
    {
        Console.WriteLine($"Dodano kategorie o id: {result.Id}");
        var updateDto = new UpdateCategoryDto(result.Id, "ZmodyfikowanaKategoria2");
        var UpdateResult = categoryServices.UpdateCategory(updateDto);

        UpdateResult.Match(
            _ => { Console.WriteLine("Pomyslna aktualizacja kategorii"); return Unit.Instance; },
            fail => { Console.WriteLine($"Nie udalo sie zaaktualizowac kategorii: {fail}"); return Unit.Instance; }
        );
        return Unit.Instance;
    },
    fail => { Console.WriteLine($"Blad podczas dodawania kategorii {fail}"); return Unit.Instance; }
);

Console.WriteLine("Test zad 4");

var pracaCategory = context.Categories.FirstOrDefault(c => c.Name == "Praca");
var pracaCategoryId = pracaCategory?.Id ?? Guid.Empty;

// proba usuniecia kategorii, do ktorej jest przypisany email (powinno wyrzucic blad)
var removeCategoryResult = categoryServices.RemoveCategory(pracaCategoryId);
removeCategoryResult.Match(
    _ => { Console.WriteLine("Usunieto kategorie."); return Unit.Instance; },
    fail => { Console.WriteLine($"Blad usuwania kategorii: {fail}"); return Unit.Instance; }
);

Console.WriteLine("Test zad 5");

// edycja kontaktu
var contactId = addContactResult.Match(v => v.Id, _ => Guid.Empty);
var updateContactDto = new UpdateContactDto(contactId, "Jan", "Nowak", Sex.Male, 35);
var updateContactResult = contactServices.UpdateContact(updateContactDto);

updateContactResult.Match(
    _ => { Console.WriteLine("Pomyslna edycja kontaktu"); return Unit.Instance; },
    fail => { Console.WriteLine($"Blad edycji kontaktu: {fail}"); return Unit.Instance; }
);



Console.WriteLine("Test zad 7");
// Dodawanie numeru telefonu
var addPhoneDto = new AddPhoneDto(contactId, "+48123456789", pracaCategoryId);
var addPhoneResult = contactServices.AddPhone(addPhoneDto);
addPhoneResult.Match(
    _ => { Console.WriteLine("Pomyslnie dodano telefon"); return Unit.Instance; },
    fail => { Console.WriteLine($"Blad dodawania telefonu: {fail}"); return Unit.Instance; }
);

// Usuwanie numeru telefonu
var removePhoneDto = new RemovePhoneDto(contactId, "+48123456789");
var removePhoneResult = contactServices.RemovePhone(removePhoneDto);
removePhoneResult.Match(
    _ => { Console.WriteLine("Pomyslnie usunieto telefon"); return Unit.Instance; },
    fail => { Console.WriteLine($"Blad usuwania telefonu: {fail}"); return Unit.Instance; }
);


Console.WriteLine("Test zad 8");

// Dodawanie ważnej daty
var addDateDto = new AddImportantDateDto(contactId, "Urodziny", new DateTime(1998, 5, 15));
var addDateResult = contactServices.AddImportantDate(addDateDto);
addDateResult.Match(
    _ => { Console.WriteLine("Pomyslnie dodano wazna date"); return Unit.Instance; },
    fail => { Console.WriteLine($"Blad dodawania daty: {fail}"); return Unit.Instance; }
);

// Usuwanie ważnej daty
var removeDateDto = new RemoveImportantDateDto(contactId, "Urodziny");
var removeDateResult = contactServices.RemoveImportantDate(removeDateDto);
removeDateResult.Match(
    _ => { Console.WriteLine("Pomyslnie usunieto wazna date"); return Unit.Instance; },
    fail => { Console.WriteLine($"Blad usuwania daty: {fail}"); return Unit.Instance; }
);

Console.WriteLine("Test zad 6 - ostatnie, bo to usuwanie kontaktu");

// usuwanie adresu email z kontaktu
var removeEmailDto = new RemoveEmailDto(contactId, "jan.kowalski@example.com");
var removeEmailResult = contactServices.RemoveEmail(removeEmailDto);

removeEmailResult.Match(
    _ => { Console.WriteLine("Pomyslnie usunieto email z kontaktu"); return Unit.Instance; },
    fail => { Console.WriteLine($"Blad usuwania emaila: {fail}"); return Unit.Instance; }
);


var removeContactResult = contactServices.RemoveContact(contactId);
removeContactResult.Match(
    _ => { Console.WriteLine("Pomyslnie usunieto kontakt"); return Unit.Instance; },
    fail => { Console.WriteLine($"Blad usuwania kontaktu: {fail}"); return Unit.Instance; }
);
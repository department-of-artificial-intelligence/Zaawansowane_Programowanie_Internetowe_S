using Microsoft.EntityFrameworkCore;
using Application.Domain;
using Application.DomainServices;
using Application.OutputPorts;
using Application.Queries;
using Application.Queries.Dtos;
using Application.Repositories;
using Application.Services;
using Application.UseCases;
using Application.UseCases.Dtos;
using Application.Utilities;
using Data;

// Przygotowanie konfiguratora naszej aplikacji
var builder = WebApplication.CreateBuilder(args);

// Dodanie usług koniecznych dla OpenApi
builder.Services.AddOpenApi();

builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<WebAPI.GlobalExceptionHandler>();

// Dodanie kontekstu danych
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("default"))
);

builder.Services.AddScoped<ICategoryQueries, CategoryQueries>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<IContactRepository, ContactRepository>();
builder.Services.AddScoped<ICategoryUseCases, CategoryServices>();
builder.Services.AddScoped<IContactUseCases, ContactServices>();
builder.Services.AddScoped<CategoryUniquenessChecker>();

// Zbudowanie aplikacji na podstawie konfiguracji
var app = builder.Build();

app.UseExceptionHandler();

// Mapowanie endpoint OpenApi oraz Swagger UI (tylko w przypadku trybu deweloperskiego).
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/openapi/v1.json", "API v1");
    });
}

// Uruchomienie przekierowania żądań z HTTP do HTTPS
app.UseHttpsRedirection();

// Zadanie 4 - grupowanie akcji wg ścieżki i tagu.
var categories = app
    .MapGroup("/api/categories")
    .WithTags("Kategorie")
    .ProducesProblem(StatusCodes.Status500InternalServerError);

categories.MapGet("/",
        (ICategoryQueries queries) => Results.Ok(queries.GetCategories()))
    .WithSummary("Pobiera wszystkie kategorie")
    .WithDescription("Zwraca listę wszystkich kategorii.")
    .Produces<IEnumerable<CategoryDto>>(StatusCodes.Status200OK);

categories.MapGet("/{categoryId:guid}",
        (Guid categoryId, ICategoryQueries queries) =>
            queries
                .GetCategory(categoryId)
                .Match(
                    category => Results.Ok(category),
                    () => Results.NotFound()))
    .WithSummary("Pobiera kategorię")
    .WithDescription("Zwraca kategorię o podanym identyfikatorze.")
    .Produces<CategoryDto>(StatusCodes.Status200OK)
    .Produces(StatusCodes.Status404NotFound);

categories.MapPost("/",
        (AddCategoryDto dto, ICategoryUseCases useCases) =>
            useCases
                .AddCategory(dto)
                .Match(
                    category => Results.Created(
                        $"/api/categories/{category.Id}",
                        category),
                    error => Results.Problem(detail: error, statusCode: 400)
                ))
    .WithSummary("Dodaje kategorię")
    .WithDescription("Tworzy nową kategorię i zwraca utworzony obiekt. Nazwa musi być niepusta i unikalna.")
    .Produces<AddCategoryResult>(StatusCodes.Status201Created)
    .ProducesProblem(StatusCodes.Status400BadRequest);

categories.MapPut("/{categoryId:guid}",
        (Guid categoryId, NameRequest request, ICategoryUseCases useCases) =>
            NoContent(useCases.RenameCategory(categoryId, request.Name)))
    .WithSummary("Zmienia nazwę kategorii")
    .WithDescription("Zmienia nazwę kategorii. Nowa nazwa musi być niepusta i unikalna.")
    .Produces(StatusCodes.Status204NoContent)
    .ProducesProblem(StatusCodes.Status400BadRequest);

categories.MapDelete("/{categoryId:guid}",
        (Guid categoryId, ICategoryUseCases useCases) =>
            NoContent(useCases.RemoveCategory(categoryId)))
    .WithSummary("Usuwa kategorię")
    .WithDescription("Usuwa kategorię. Nie można usunąć kategorii przypisanej do adresu email.")
    .Produces(StatusCodes.Status204NoContent)
    .ProducesProblem(StatusCodes.Status400BadRequest);

var contacts = app
    .MapGroup("/api/contacts")
    .WithTags("Kontakty")
    .ProducesProblem(StatusCodes.Status500InternalServerError);

contacts.MapPost("/",
        (AddContactDto dto, IContactUseCases useCases) =>
            useCases
                .AddContact(dto)
                .Match(
                    contact => Results.Created(
                        $"/api/contacts/{contact.Id}",
                        contact),
                    error => Results.Problem(detail: error, statusCode: 400)
                ))
    .WithSummary("Dodaje kontakt")
    .WithDescription("Tworzy nowy kontakt i zwraca utworzony obiekt. " +
                     "Płeć: 0 - Male, 1 - Female. Wiek musi być z zakresu 18-120.")
    .Produces<AddContactResult>(StatusCodes.Status201Created)
    .ProducesProblem(StatusCodes.Status400BadRequest);

contacts.MapPut("/{contactId:guid}",
        (Guid contactId, UpdateContactRequest request, IContactUseCases useCases) =>
            NoContent(useCases.UpdateContact(new UpdateContactDto(
                contactId, request.FirstName, request.LastName, request.Sex, request.Age))))
    .WithSummary("Modyfikuje kontakt")
    .WithDescription("Zmienia imię, nazwisko, płeć i wiek kontaktu.")
    .Produces(StatusCodes.Status204NoContent)
    .ProducesProblem(StatusCodes.Status400BadRequest);

contacts.MapDelete("/{contactId:guid}",
        (Guid contactId, IContactUseCases useCases) =>
            NoContent(useCases.RemoveContact(contactId)))
    .WithSummary("Usuwa kontakt")
    .WithDescription("Usuwa kontakt wraz z jego adresami email, telefonami i datami.")
    .Produces(StatusCodes.Status204NoContent)
    .ProducesProblem(StatusCodes.Status400BadRequest);

contacts.MapPost("/{contactId:guid}/emails",
        (Guid contactId, AddEmailRequest request, IContactUseCases useCases) =>
            NoContent(useCases.AddEmail(new AddEmailDto(contactId, request.Address, request.CategoryId))))
    .WithSummary("Dodaje adres email do kontaktu")
    .WithDescription("Dołącza do kontaktu adres email przypisany do istniejącej kategorii.")
    .Produces(StatusCodes.Status204NoContent)
    .ProducesProblem(StatusCodes.Status400BadRequest);

contacts.MapDelete("/{contactId:guid}/emails/{address}",
        (Guid contactId, string address, IContactUseCases useCases) =>
            NoContent(useCases.RemoveEmail(contactId, address)))
    .WithSummary("Usuwa adres email z kontaktu")
    .WithDescription("Usuwa z kontaktu podany adres email.")
    .Produces(StatusCodes.Status204NoContent)
    .ProducesProblem(StatusCodes.Status400BadRequest);

contacts.MapPost("/{contactId:guid}/phones",
        (Guid contactId, PhoneRequest request, IContactUseCases useCases) =>
            NoContent(useCases.AddPhone(contactId, request.Number)))
    .WithSummary("Dodaje numer telefonu do kontaktu")
    .WithDescription("Dołącza do kontaktu numer telefonu (9-15 cyfr, opcjonalnie z prefiksem +).")
    .Produces(StatusCodes.Status204NoContent)
    .ProducesProblem(StatusCodes.Status400BadRequest);

contacts.MapDelete("/{contactId:guid}/phones/{number}",
        (Guid contactId, string number, IContactUseCases useCases) =>
            NoContent(useCases.RemovePhone(contactId, number)))
    .WithSummary("Usuwa numer telefonu z kontaktu")
    .WithDescription("Usuwa z kontaktu podany numer telefonu.")
    .Produces(StatusCodes.Status204NoContent)
    .ProducesProblem(StatusCodes.Status400BadRequest);

contacts.MapPost("/{contactId:guid}/important-dates",
        (Guid contactId, ImportantDateRequest request, IContactUseCases useCases) =>
            NoContent(useCases.AddImportantDate(contactId, request.Date, request.Description)))
    .WithSummary("Dodaje ważną datę do kontaktu")
    .WithDescription("Dołącza do kontaktu datę z opisem (np. urodziny). Opis musi być unikalny w kontakcie.")
    .Produces(StatusCodes.Status204NoContent)
    .ProducesProblem(StatusCodes.Status400BadRequest);

contacts.MapDelete("/{contactId:guid}/important-dates/{description}",
        (Guid contactId, string description, IContactUseCases useCases) =>
            NoContent(useCases.RemoveImportantDate(contactId, description)))
    .WithSummary("Usuwa ważną datę z kontaktu")
    .WithDescription("Usuwa z kontaktu datę o podanym opisie.")
    .Produces(StatusCodes.Status204NoContent)
    .ProducesProblem(StatusCodes.Status400BadRequest);

// Uruchomienie aplikacji
app.Run();

// Wynik komendy niezwracającej danych: 204 przy sukcesie, 400 z opisem błędu w przeciwnym razie.
static IResult NoContent(Result<Unit> result) =>
    result.Match(
        _ => Results.NoContent(),
        error => Results.Problem(detail: error, statusCode: 400));

// Treści żądań, w których identyfikator jest przekazywany w ścieżce.
record NameRequest(string Name);
record UpdateContactRequest(string FirstName, string LastName, Sex Sex, int Age);
record AddEmailRequest(string Address, Guid CategoryId);
record PhoneRequest(string Number);
record ImportantDateRequest(DateOnly Date, string Description);

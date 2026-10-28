using Microsoft.EntityFrameworkCore;
using Application.DomainServices;
using Application.OutputPorts;
using Application.Queries;
using Application.Queries.Dtos;
using Application.Repositories;
using Application.Services;
using Application.UseCases;
using Application.UseCases.Dtos;
using Data;

// Przygotowanie konfiguratora naszej aplikacji
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("default"))
);

builder.Services.AddScoped<ICategoryQueries, CategoryQueries>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<ICategoryUseCases, CategoryServices>();
builder.Services.AddScoped<CategoryUniquenessChecker>();
builder.Services.AddScoped<IContactRepository, ContactRepository>();
builder.Services.AddScoped<IContactUseCases, ContactServices>();

builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<WebAPI.GlobalExceptionHandler>();

// Dodanie usług koniecznych dla OpenApi
builder.Services.AddOpenApi();

// Zbudowanie aplikacji na podstawie konfiguracji
var app = builder.Build();

app.UseExceptionHandler();

// Mapowanie endpoint OpenApi (tylko w przypadku trybu deweloperskiego).
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

var categories = app.MapGroup("/api/categories").WithTags("Kategorie");

categories.MapPost("/",
    (AddCategoryDto dto, ICategoryUseCases useCases) =>
        useCases
            .AddCategory(dto)
            .Match(
                category => Results.Created(
                    $"/api/categories/{category.Id}",
                    category),
                error => Results.Problem(detail: error, statusCode: 400)
            )
)
.WithSummary("Dodaje kategorię")
.WithDescription("Tworzy nową kategorię i zwraca utworzony obiekt.")
.Produces<AddCategoryResult>(StatusCodes.Status201Created)
.Produces(StatusCodes.Status400BadRequest);

categories.MapGet("/",
    (ICategoryQueries queries) => Results.Ok(queries.GetCategories())
)
.WithSummary("Pobiera wszystkie kategorie")
.WithDescription("Zwraca listę wszystkich kategorii.")
.Produces<IEnumerable<CategoryDto>>(StatusCodes.Status200OK);

categories.MapGet("/{id:guid}",
    (Guid id, ICategoryQueries queries) =>
        queries
            .GetCategory(id)
            .Match<IResult>(
                category => Results.Ok(category),
                () => Results.NotFound()
            )
)
.WithSummary("Pobiera kategorię")
.WithDescription("Zwraca kategorię o podanym identyfikatorze.")
.Produces<CategoryDto>(StatusCodes.Status200OK)
.Produces(StatusCodes.Status404NotFound);

categories.MapPut("/",
    (UpdateCategoryDto dto, ICategoryUseCases useCases) =>
        useCases
            .UpdateCategory(dto)
            .Match(
                _ => Results.NoContent(),
                error => Results.Problem(detail: error, statusCode: 400)
            )
)
.WithSummary("Zmienia nazwę kategorii")
.WithDescription("Zmienia nazwę istniejącej kategorii na podaną.")
.Produces(StatusCodes.Status204NoContent)
.Produces(StatusCodes.Status400BadRequest);

categories.MapDelete("/{id:guid}",
    (Guid id, ICategoryUseCases useCases) =>
        useCases
            .RemoveCategory(id)
            .Match(
                _ => Results.NoContent(),
                error => Results.Problem(detail: error, statusCode: 400)
            )
)
.WithSummary("Usuwa kategorię")
.WithDescription("Usuwa kategorię, o ile nie jest przypisana do żadnego adresu e-mail.")
.Produces(StatusCodes.Status204NoContent)
.Produces(StatusCodes.Status400BadRequest);

app.MapPost("/api/contacts",
    (AddContactDto dto, IContactUseCases useCases) =>
        useCases
            .AddContact(dto)
            .Match(
                contact => Results.Created(
                    $"/api/contacts/{contact.Id}",
                    contact),
                error => Results.Problem(detail: error, statusCode: 400)
            )
)
.WithSummary("Dodaje kontakt")
.WithDescription("Tworzy nowy kontakt i zwraca utworzony obiekt.")
.WithTags("Kontakty")
.Produces<AddContactResult>(StatusCodes.Status201Created)
.Produces(StatusCodes.Status400BadRequest);

app.MapPut("/api/contacts",
    (UpdateContactDto dto, IContactUseCases useCases) =>
        useCases
            .UpdateContact(dto)
            .Match(
                _ => Results.NoContent(),
                error => Results.Problem(detail: error, statusCode: 400)
            )
)
.WithSummary("Zmienia dane kontaktu")
.WithDescription("Zmienia imię, nazwisko, płeć i wiek istniejącego kontaktu.")
.WithTags("Kontakty")
.Produces(StatusCodes.Status204NoContent)
.Produces(StatusCodes.Status400BadRequest);

app.MapDelete("/api/contacts/{id:guid}",
    (Guid id, IContactUseCases useCases) =>
        useCases
            .RemoveContact(id)
            .Match(
                _ => Results.NoContent(),
                error => Results.Problem(detail: error, statusCode: 400)
            )
)
.WithSummary("Usuwa kontakt")
.WithDescription("Usuwa kontakt razem z jego adresami e-mail, numerami telefonu i ważnymi datami.")
.WithTags("Kontakty")
.Produces(StatusCodes.Status204NoContent)
.Produces(StatusCodes.Status400BadRequest);

app.MapPost("/api/contacts/emails",
    (AddEmailDto dto, IContactUseCases useCases) =>
        useCases
            .AddEmail(dto)
            .Match(
                _ => Results.NoContent(),
                error => Results.Problem(detail: error, statusCode: 400)
            )
)
.WithSummary("Dodaje adres e-mail do kontaktu")
.WithDescription("Dołącza do kontaktu adres e-mail przypisany do wskazanej kategorii.")
.WithTags("Kontakty")
.Produces(StatusCodes.Status204NoContent)
.Produces(StatusCodes.Status400BadRequest);

app.MapDelete("/api/contacts/{contactId:guid}/emails/{address}",
    (Guid contactId, string address, IContactUseCases useCases) =>
        useCases
            .RemoveEmail(new RemoveEmailDto(contactId, address))
            .Match(
                _ => Results.NoContent(),
                error => Results.Problem(detail: error, statusCode: 400)
            )
)
.WithSummary("Usuwa adres e-mail z kontaktu")
.WithDescription("Usuwa z kontaktu podany adres e-mail.")
.WithTags("Kontakty")
.Produces(StatusCodes.Status204NoContent)
.Produces(StatusCodes.Status400BadRequest);

app.MapPost("/api/contacts/phones",
    (AddPhoneNumberDto dto, IContactUseCases useCases) =>
        useCases
            .AddPhoneNumber(dto)
            .Match(
                _ => Results.NoContent(),
                error => Results.Problem(detail: error, statusCode: 400)
            )
)
.WithSummary("Dodaje numer telefonu do kontaktu")
.WithDescription("Dołącza do kontaktu numer telefonu.")
.WithTags("Kontakty")
.Produces(StatusCodes.Status204NoContent)
.Produces(StatusCodes.Status400BadRequest);

app.MapDelete("/api/contacts/{contactId:guid}/phones/{number}",
    (Guid contactId, string number, IContactUseCases useCases) =>
        useCases
            .RemovePhoneNumber(new RemovePhoneNumberDto(contactId, number))
            .Match(
                _ => Results.NoContent(),
                error => Results.Problem(detail: error, statusCode: 400)
            )
)
.WithSummary("Usuwa numer telefonu z kontaktu")
.WithDescription("Usuwa z kontaktu podany numer telefonu.")
.WithTags("Kontakty")
.Produces(StatusCodes.Status204NoContent)
.Produces(StatusCodes.Status400BadRequest);

app.MapPost("/api/contacts/important-dates",
    (AddImportantDateDto dto, IContactUseCases useCases) =>
        useCases
            .AddImportantDate(dto)
            .Match(
                _ => Results.NoContent(),
                error => Results.Problem(detail: error, statusCode: 400)
            )
)
.WithSummary("Dodaje ważną datę do kontaktu")
.WithDescription("Dołącza do kontaktu ważną datę z opisem, np. urodziny.")
.WithTags("Kontakty")
.Produces(StatusCodes.Status204NoContent)
.Produces(StatusCodes.Status400BadRequest);

app.MapDelete("/api/contacts/{contactId:guid}/important-dates",
    (Guid contactId, string description, DateOnly date, IContactUseCases useCases) =>
        useCases
            .RemoveImportantDate(
                new RemoveImportantDateDto(contactId, description, date))
            .Match(
                _ => Results.NoContent(),
                error => Results.Problem(detail: error, statusCode: 400)
            )
)
.WithSummary("Usuwa ważną datę z kontaktu")
.WithDescription("Usuwa z kontaktu ważną datę o podanym opisie i dniu.")
.WithTags("Kontakty")
.Produces(StatusCodes.Status204NoContent)
.Produces(StatusCodes.Status400BadRequest);

// Uruchomienie aplikacji
app.Run();

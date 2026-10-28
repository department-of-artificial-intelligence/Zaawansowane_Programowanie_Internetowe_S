using Application.DomainServices;
using Application.OutputPorts;
using Application.Queries;
using Application.Repositories;
using Application.Services;
using Application.UseCases;
using Application.UseCases.Dtos;
using Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// 1. Rejestracja bazy danych SQLite
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("default"))
);

// 2. Rejestracja serwisów i repozytoriów w kontenerze DI
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

builder.Services.AddScoped<ICategoryQueries, CategoryQueries>();
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<ICategoryUseCases, CategoryServices>();
builder.Services.AddScoped<CategoryUniquenessChecker>();

builder.Services.AddScoped<IContactRepository, ContactRepository>();
builder.Services.AddScoped<IContactUseCases, ContactServices>();

// 3. Obsługa błędów Problem Details i OpenAPI
builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<WebAPI.GlobalExceptionHandler>();
builder.Services.AddOpenApi();

var app = builder.Build();

app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/openapi/v1.json", "API v1");
    });
}

// ============================================================================
// 1. MODUŁ KATEGORII (Zgrupowany za pomocą MapGroup)
// ============================================================================

var categories = app.MapGroup("/api/categories")
    .WithTags("Kategorie");

// GET /api/categories
categories.MapGet("", (ICategoryQueries queries) =>
    Results.Ok(queries.GetCategories())
)
.WithSummary("Pobiera wszystkie kategorie")
.WithDescription("Zwraca pełną listę zdefiniowanych kategorii kontaktów.")
.Produces(StatusCodes.Status200OK);

// POST /api/categories
categories.MapPost("", (AddCategoryDto dto, ICategoryUseCases useCases) =>
    useCases.AddCategory(dto).Match(
        category => Results.Created($"/api/categories/{category.Id}", category),
        error => Results.Problem(detail: error, statusCode: 400)
    )
)
.WithSummary("Dodaje nową kategorię")
.WithDescription("Tworzy nową kategorię kontaktów i zwraca utworzony obiekt.")
.Produces<AddCategoryResult>(StatusCodes.Status201Created)
.Produces(StatusCodes.Status400BadRequest);

// PUT /api/categories/{id:guid}
categories.MapPut("/{id:guid}", (Guid id, UpdateCategoryDto dto, ICategoryUseCases useCases) =>
{
    var updateDto = new UpdateCategoryDto(id, dto.Name);
    return useCases.UpdateCategory(updateDto).Match(
        _ => Results.NoContent(),
        error => Results.Problem(detail: error, statusCode: 400)
    );
})
.WithSummary("Modyfikuje kategorię")
.WithDescription("Zmienia nazwę istniejącej kategorii o podanym identyfikatorze GUID.")
.Produces(StatusCodes.Status204NoContent)
.Produces(StatusCodes.Status400BadRequest);

// DELETE /api/categories/{id:guid}
categories.MapDelete("/{id:guid}", (Guid id, ICategoryUseCases useCases) =>
    useCases.RemoveCategory(id).Match(
        _ => Results.NoContent(),
        error => Results.Problem(detail: error, statusCode: 400)
    )
)
.WithSummary("Usuwa kategorię")
.WithDescription("Usuwa wybraną kategorię pod warunkiem, że nie jest przypisana do żadnego kontaktu.")
.Produces(StatusCodes.Status204NoContent)
.Produces(StatusCodes.Status400BadRequest);

// ============================================================================
// 2. MODUŁ KONTAKTÓW
// ============================================================================

app.MapPost("/api/contacts", (AddContactDto dto, IContactUseCases useCases) =>
    useCases.AddContact(dto).Match(
        contact => Results.Created($"/api/contacts/{contact.Id}", contact),
        error => Results.Problem(detail: error, statusCode: 400)
    )
)
.WithSummary("Tworzy nowy kontakt")
.WithDescription("Rejestruje nowy kontakt z podstawowymi danymi osobowymi (imię, nazwisko, płeć, wiek).")
.WithTags("Kontakty")
.Produces<AddContactResult>(StatusCodes.Status201Created)
.Produces(StatusCodes.Status400BadRequest);

app.MapPut("/api/contacts/{id:guid}", (Guid id, UpdateContactDto dto, IContactUseCases useCases) =>
{
    var updateDto = new UpdateContactDto(id, dto.FirstName, dto.LastName, dto.Sex, dto.Age);
    return useCases.UpdateContact(updateDto).Match(
        _ => Results.NoContent(),
        error => Results.Problem(detail: error, statusCode: 400)
    );
})
.WithSummary("Aktualizuje dane kontaktu")
.WithDescription("Modyfikuje dane osobowe istniejącego kontaktu.")
.WithTags("Kontakty")
.Produces(StatusCodes.Status204NoContent)
.Produces(StatusCodes.Status400BadRequest);

app.MapDelete("/api/contacts/{id:guid}", (Guid id, IContactUseCases useCases) =>
    useCases.RemoveContact(id).Match(
        _ => Results.NoContent(),
        error => Results.Problem(detail: error, statusCode: 400)
    )
)
.WithSummary("Usuwa kontakt")
.WithDescription("Usuwa kontakt oraz kaskadowo wszystkie przypisane do niego telefony, adresy e-mail i daty.")
.WithTags("Kontakty")
.Produces(StatusCodes.Status204NoContent)
.Produces(StatusCodes.Status400BadRequest);

// ============================================================================
// 3. PODZASOBY: ADRESY E-MAIL
// ============================================================================

app.MapPost("/api/contacts/{contactId:guid}/emails", (Guid contactId, AddEmailRequest request, IContactUseCases useCases) =>
{
    var dto = new AddEmailDto(contactId, request.Address, request.CategoryId);
    return useCases.AddEmail(dto).Match(
        _ => Results.Ok(),
        error => Results.Problem(detail: error, statusCode: 400)
    );
})
.WithSummary("Dodaje adres e-mail")
.WithDescription("Dołącza do kontaktu nowy adres e-mail przypisany do konkretnej kategorii.")
.WithTags("Kontakty - E-maile")
.Produces(StatusCodes.Status200OK)
.Produces(StatusCodes.Status400BadRequest);

app.MapDelete("/api/contacts/{contactId:guid}/emails", (Guid contactId, string address, IContactUseCases useCases) =>
{
    var dto = new RemoveEmailDto(contactId, address);
    return useCases.RemoveEmail(dto).Match(
        _ => Results.NoContent(),
        error => Results.Problem(detail: error, statusCode: 400)
    );
})
.WithSummary("Usuwa adres e-mail")
.WithDescription("Usuwa wskazany adres e-mail z podanego kontaktu.")
.WithTags("Kontakty - E-maile")
.Produces(StatusCodes.Status204NoContent)
.Produces(StatusCodes.Status400BadRequest);

// ============================================================================
// 4. PODZASOBY: NUMERY TELEFONÓW
// ============================================================================

app.MapPost("/api/contacts/{contactId:guid}/phones", (Guid contactId, AddPhoneRequest request, IContactUseCases useCases) =>
{
    var dto = new AddPhoneDto(contactId, request.Number, request.CategoryId);
    return useCases.AddPhone(dto).Match(
        _ => Results.Ok(),
        error => Results.Problem(detail: error, statusCode: 400)
    );
})
.WithSummary("Dodaje numer telefonu")
.WithDescription("Dołącza do kontaktu numer telefonu przypisany do określonej kategorii.")
.WithTags("Kontakty - Telefony")
.Produces(StatusCodes.Status200OK)
.Produces(StatusCodes.Status400BadRequest);

app.MapDelete("/api/contacts/{contactId:guid}/phones", (Guid contactId, string number, IContactUseCases useCases) =>
{
    var dto = new RemovePhoneDto(contactId, number);
    return useCases.RemovePhone(dto).Match(
        _ => Results.NoContent(),
        error => Results.Problem(detail: error, statusCode: 400)
    );
})
.WithSummary("Usuwa numer telefonu")
.WithDescription("Usuwa wskazany numer telefonu z kontaktu.")
.WithTags("Kontakty - Telefony")
.Produces(StatusCodes.Status204NoContent)
.Produces(StatusCodes.Status400BadRequest);

// ============================================================================
// 5. PODZASOBY: WAŻNE DATY
// ============================================================================

app.MapPost("/api/contacts/{contactId:guid}/dates", (Guid contactId, AddImportantDateRequest request, IContactUseCases useCases) =>
{
    var dto = new AddImportantDateDto(contactId, request.Date, request.Description);
    return useCases.AddImportantDate(dto).Match(
        _ => Results.Ok(),
        error => Results.Problem(detail: error, statusCode: 400)
    );
})
.WithSummary("Dodaje ważną datę")
.WithDescription("Zapisuje powiązaną z kontaktem ważną datę wraz z jej opisem.")
.WithTags("Kontakty - Ważne daty")
.Produces(StatusCodes.Status200OK)
.Produces(StatusCodes.Status400BadRequest);

app.MapDelete("/api/contacts/{contactId:guid}/dates", (Guid contactId, string description, IContactUseCases useCases) =>
{
    var dto = new RemoveImportantDateDto(contactId, description);
    return useCases.RemoveImportantDate(dto).Match(
        _ => Results.NoContent(),
        error => Results.Problem(detail: error, statusCode: 400)
    );
})
.WithSummary("Usuwa ważną datę")
.WithDescription("Usuwa wpis o dacie z kontaktu na podstawie jej opisu.")
.WithTags("Kontakty - Ważne daty")
.Produces(StatusCodes.Status204NoContent)
.Produces(StatusCodes.Status400BadRequest);

app.Run();

// Obiekty pomocnicze do przesyłania ciał żądań podzasobów
public record AddEmailRequest(string Address, Guid CategoryId);
public record AddPhoneRequest(string Number, Guid CategoryId);
public record AddImportantDateRequest(DateOnly Date, string Description);
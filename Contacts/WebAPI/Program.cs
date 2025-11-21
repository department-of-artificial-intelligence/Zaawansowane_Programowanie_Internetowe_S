using Microsoft.EntityFrameworkCore;
using Contacts.Application;
using Contacts.Application.Queries;
using Contacts.Application.Repositories;
using Contacts.Application.Services;
using Contacts.Application.UseCases;
using Contacts.Application.UseCases.DTOs;
using Contacts.Data;
using Contacts.Application.Domain; // Potrzebne dla Sex i DateTime

// --- 1. Konfiguracja Buildera ---
var builder = WebApplication.CreateBuilder(args);

// --- 2. Rejestracja Usług (Dependency Injection) ---

// Dodanie DbContext
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("default"))
);

// Używamy AddScoped dla wszystkich serwisów i repozytoriów
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<IContactRepository, ContactRepository>();
builder.Services.AddScoped<ICategoryQueries, CategoriesQueries>();
builder.Services.AddScoped<ICategoryUseCases, CategoryServices>();
builder.Services.AddScoped<IContactUseCases, ContactServices>();

// Dodanie usług dla Swaggera
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


// --- 3. Budowanie Aplikacji ---
var app = builder.Build();

// --- 4. Konfiguracja Pipeline'u HTTP ---

// Uruchomienie Swaggera w trybie deweloperskim
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// --- 5. Implementacja Endpointów (Zadanie 1 i 2 z PDF) ---

// Grupa 1: Kategorie
var categoryGroup = app.MapGroup("/api/categories").WithTags("Kategorie");

// GET /api/categories
categoryGroup.MapGet("/", (ICategoryQueries queries) =>
{
    try
    {
        return Results.Ok(queries.GetCategories());
    }
    catch (Exception ex)
    {
        return Results.Problem(ex.Message);
    }
})
.WithName("GetAllCategories")
.WithSummary("Pobiera wszystkie kategorie")
.WithDescription("Zwraca listę wszystkich kategorii z bazy danych.")
.Produces<IEnumerable<CategoryDTO>>(StatusCodes.Status200OK);

// POST /api/categories
categoryGroup.MapPost("/", (AddCategoryDTO dto, ICategoryUseCases useCases) =>
{
    try
    {
        var result = useCases.AddCategory(dto);
        // Zwracamy 201 Created z lokalizacją nowego zasobu i samym zasobem
        return Results.Created($"/api/categories/{result.Id}", result);
    }
    catch (ServiceException ex)
    {
        return Results.Problem(ex.Message, statusCode: 400); // Błąd biznesowy (np. duplikat)
    }
    catch (Exception)
    {
        return Results.Problem("Wystąpił nieoczekiwany błąd", statusCode: 500);
    }
})
.WithName("AddCategory")
.WithSummary("Tworzy nową kategorię")
.WithDescription("Tworzy nową kategorię na podstawie podanej nazwy.")
.Produces<AddCategoryResult>(StatusCodes.Status201Created)
.Produces(StatusCodes.Status400BadRequest)
.Produces(StatusCodes.Status500InternalServerError);

// PUT /api/categories/{id}
categoryGroup.MapPut("/{id}", (int id, UpdateCategoryDTO dto, ICategoryUseCases useCases) =>
{
    try
    {
        dto.Id = id;
        useCases.UpdateCategory(dto);
        return Results.NoContent(); // 204 No Content to standardowa odpowiedź dla PUT
    }
    catch (ServiceException ex)
    {
        return Results.Problem(ex.Message, statusCode: 404); // Not Found
    }
    catch (Exception)
    {
        return Results.Problem("Wystąpił nieoczekiwany błąd", statusCode: 500);
    }
})
.WithName("UpdateCategory")
.WithSummary("Aktualizuje istniejącą kategorię")
.Produces(StatusCodes.Status204NoContent)
.Produces(StatusCodes.Status404NotFound)
.Produces(StatusCodes.Status500InternalServerError);

// DELETE /api/categories/{id}
categoryGroup.MapDelete("/{id}", (int id, ICategoryUseCases useCases) =>
{
    try
    {
        useCases.RemoveCategory(id);
        return Results.NoContent(); // 204 No Content
    }
    catch (ServiceException ex)
    {
        return Results.Problem(ex.Message, statusCode: 404); // Not Found lub 400 Bad Request
    }
    catch (Exception)
    {
        return Results.Problem("Wystąpił nieoczekiwany błąd", statusCode: 500);
    }
})
.WithName("RemoveCategory")
.WithSummary("Usuwa kategorię")
.Produces(StatusCodes.Status204NoContent)
.Produces(StatusCodes.Status404NotFound)
.Produces(StatusCodes.Status500InternalServerError);


// Grupa 2: Kontakty
var contactGroup = app.MapGroup("/api/contacts").WithTags("Kontakty");

// POST /api/contacts
contactGroup.MapPost("/", async (AddContactDTO dto, IContactUseCases useCases) =>
{
    try
    {
        await useCases.AddContactAsync(dto);
        return Results.Ok("Kontakt został pomyślnie utworzony."); // Dla uproszczenia zwracamy 200 OK
    }
    catch (Exception ex)
    {
        return Results.Problem(ex.Message, statusCode: 500);
    }
})
.WithName("AddContact")
.WithSummary("Tworzy nowy kontakt")
.Produces(StatusCodes.Status200OK)
.Produces(StatusCodes.Status500InternalServerError);

// DELETE /api/contacts/{id}
contactGroup.MapDelete("/{id}", async (int id, IContactUseCases useCases) =>
{
    try
    {
        await useCases.RemoveContactAsync(id);
        return Results.NoContent();
    }
    catch (ServiceException ex)
    {
        return Results.Problem(ex.Message, statusCode: 404);
    }
    catch (Exception ex)
    {
        return Results.Problem(ex.Message, statusCode: 500);
    }
})
.WithName("RemoveContact")
.WithSummary("Usuwa kontakt i wszystkie jego dane")
.Produces(StatusCodes.Status204NoContent)
.Produces(StatusCodes.Status404NotFound)
.Produces(StatusCodes.Status500InternalServerError);

// PUT /api/contacts/{id}
contactGroup.MapPut("/{id}", async (int id, UpdateContactDTO dto, IContactUseCases useCases) =>
{
    try
    {
        dto.Id = id;
        await useCases.UpdateContactAsync(dto);
        return Results.NoContent();
    }
    catch (ServiceException ex)
    {
        return Results.Problem(ex.Message, statusCode: 404);
    }
    catch (Exception ex)
    {
        return Results.Problem(ex.Message, statusCode: 500);
    }
})
.WithName("UpdateContactInfo")
.WithSummary("Aktualizuje podstawowe dane kontaktu")
.Produces(StatusCodes.Status204NoContent)
.Produces(StatusCodes.Status404NotFound)
.Produces(StatusCodes.Status500InternalServerError);

// POST /api/contacts/{contactId}/emails
contactGroup.MapPost("/{contactId}/emails", async (int contactId, AddEmailToContactDTO dto, IContactUseCases useCases) =>
{
    try
    {
        dto.ContactId = contactId;
        await useCases.AddEmailToContactAsync(dto);
        return Results.Ok("Email został dodany.");
    }
    catch (ServiceException ex)
    {
        return Results.Problem(ex.Message, statusCode: 404);
    }
    catch (Exception ex)
    {
        return Results.Problem(ex.Message, statusCode: 500);
    }
})
.WithName("AddEmailToContact")
.WithSummary("Dodaje nowy email do kontaktu")
.Produces(StatusCodes.Status200OK)
.Produces(StatusCodes.Status404NotFound)
.Produces(StatusCodes.Status500InternalServerError);

// DELETE /api/contacts/{contactId}/emails/{emailId}
contactGroup.MapDelete("/{contactId}/emails/{emailId}", async (int contactId, int emailId, IContactUseCases useCases) =>
{
    try
    {
        var dto = new RemoveEmailFromContactDTO { ContactId = contactId, EmailId = emailId };
        await useCases.RemoveEmailFromContactAsync(dto);
        return Results.NoContent();
    }
    catch (ServiceException ex)
    {
        return Results.Problem(ex.Message, statusCode: 404);
    }
    catch (Exception ex)
    {
        return Results.Problem(ex.Message, statusCode: 500);
    }
})
.WithName("RemoveEmailFromContact")
.WithSummary("Usuwa email z kontaktu")
.Produces(StatusCodes.Status204NoContent)
.Produces(StatusCodes.Status404NotFound)
.Produces(StatusCodes.Status500InternalServerError);

// POST /api/contacts/{contactId}/phones
contactGroup.MapPost("/{contactId}/phones", async (int contactId, AddPhoneToContactDTO dto, IContactUseCases useCases) =>
{
    try
    {
        dto.ContactId = contactId;
        await useCases.AddPhoneToContactAsync(dto);
        return Results.Ok("Telefon został dodany.");
    }
    catch (ServiceException ex) { return Results.Problem(ex.Message, statusCode: 404); }
    catch (Exception ex) { return Results.Problem(ex.Message, statusCode: 500); }
})
.WithName("AddPhoneToContact")
.WithSummary("Dodaje nowy telefon do kontaktu")
.Produces(StatusCodes.Status200OK)
.Produces(StatusCodes.Status404NotFound)
.Produces(StatusCodes.Status500InternalServerError);

// DELETE /api/contacts/{contactId}/phones/{phoneId}
contactGroup.MapDelete("/{contactId}/phones/{phoneId}", async (int contactId, int phoneId, IContactUseCases useCases) =>
{
    try
    {
        var dto = new RemovePhoneFromContactDTO { ContactId = contactId, PhoneId = phoneId };
        await useCases.RemovePhoneFromContactAsync(dto);
        return Results.NoContent();
    }
    catch (ServiceException ex) { return Results.Problem(ex.Message, statusCode: 404); }
    catch (Exception ex) { return Results.Problem(ex.Message, statusCode: 500); }
})
.WithName("RemovePhoneFromContact")
.WithSummary("Usuwa telefon z kontaktu")
.Produces(StatusCodes.Status204NoContent)
.Produces(StatusCodes.Status404NotFound)
.Produces(StatusCodes.Status500InternalServerError);


// POST /api/contacts/{contactId}/dates
contactGroup.MapPost("/{contactId}/dates", async (int contactId, AddImportantDateToContactDTO dto, IContactUseCases useCases) =>
{
    try
    {
        dto.ContactId = contactId;
        await useCases.AddImportantDateToContactAsync(dto);
        return Results.Ok("Data została dodana.");
    }
    catch (ServiceException ex) { return Results.Problem(ex.Message, statusCode: 404); }
    catch (Exception ex) { return Results.Problem(ex.Message, statusCode: 500); }
})
.WithName("AddDateToContact")
.WithSummary("Dodaje nową ważną datę do kontaktu")
.Produces(StatusCodes.Status200OK)
.Produces(StatusCodes.Status404NotFound)
.Produces(StatusCodes.Status500InternalServerError);

// DELETE /api/contacts/{contactId}/dates/{dateId}
contactGroup.MapDelete("/{contactId}/dates/{dateId}", async (int contactId, int dateId, IContactUseCases useCases) =>
{
    try
    {
        var dto = new RemoveImportantDateFromContactDTO { ContactId = contactId, ImportantDateId = dateId };
        await useCases.RemoveImportantDateFromContactAsync(dto);
        return Results.NoContent();
    }
    catch (ServiceException ex) { return Results.Problem(ex.Message, statusCode: 404); }
    catch (Exception ex) { return Results.Problem(ex.Message, statusCode: 500); }
})
.WithName("RemoveDateFromContact")
.WithSummary("Usuwa ważną datę z kontaktu")
.Produces(StatusCodes.Status204NoContent)
.Produces(StatusCodes.Status404NotFound)
.Produces(StatusCodes.Status500InternalServerError);


// --- 6. Uruchomienie Aplikacji ---
app.Run();
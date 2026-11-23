using Microsoft.EntityFrameworkCore;
using Application.DomainServices;
using Application.OutputPorts;
using Application.Queries;
using Application.Repositories;
using Application.Services;
using Application.UseCases;
using Application.UseCases.Dtos;
using Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("default")));

// Rejestracja zależności (DI)
builder.Services.AddScoped<ICategoryQueries, CategoryQueries>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<ICategoryUseCases, CategoryServices>();
builder.Services.AddScoped<CategoryUniquenessChecker>();

builder.Services.AddScoped<IContactRepository, ContactRepository>();
builder.Services.AddScoped<IContactUseCases, ContactServices>();

builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<WebAPI.GlobalExceptionHandler>();

var app = builder.Build();

app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/openapi/v1.json", "API v1");
    });
    app.MapOpenApi();
}

app.UseHttpsRedirection();

// =========================================================================
// GRUPA 1: KATEGORIE (/api/categories)
// =========================================================================
var categoriesGroup = app.MapGroup("/api/categories")
    .WithTags("Kategorie");

categoriesGroup.MapGet("/", (ICategoryQueries queries) =>
    Results.Ok(queries.GetCategories()))
    .WithSummary("Pobiera listę kategorii")
    .WithDescription("Zwraca wszystkie kategorie zapisane w bazie danych.")
    .Produces(StatusCodes.Status200OK);

categoriesGroup.MapPost("/", (AddCategoryDto dto, ICategoryUseCases useCases) =>
    useCases
        .AddCategory(dto)
        .Match(
            category => Results.Created($"/api/categories/{category.Id}", category),
            error => Results.Problem(detail: error, statusCode: 400)
        ))
    .WithSummary("Dodaje kategorię")
    .WithDescription("Tworzy nową kategorię i zwraca utworzony obiekt.")
    .Produces<AddCategoryResult>(StatusCodes.Status201Created)
    .Produces(StatusCodes.Status400BadRequest);

categoriesGroup.MapPut("/{id:guid}", (Guid id, string newName, ICategoryUseCases useCases) =>
    useCases.UpdateCategory(id, newName).Match(
        _ => Results.NoContent(),
        error => Results.Problem(detail: error, statusCode: 400)
    ))
    .WithSummary("Aktualizuje nazwę kategorii")
    .WithDescription("Zmienia nazwę istniejącej kategorii na podstawie jej identyfikatora.")
    .Produces(StatusCodes.Status204NoContent)
    .Produces(StatusCodes.Status400BadRequest);

categoriesGroup.MapDelete("/{id:guid}", (Guid id, ICategoryUseCases useCases) =>
    useCases.RemoveCategory(id).Match(
        _ => Results.NoContent(),
        error => Results.Problem(detail: error, statusCode: 400)
    ))
    .WithSummary("Usuwa kategorię")
    .WithDescription("Usuwa wybraną kategorię z bazy danych.")
    .Produces(StatusCodes.Status204NoContent)
    .Produces(StatusCodes.Status400BadRequest);

// =========================================================================
// GRUPA 2: KONTAKTY (/api/contacts)
// =========================================================================
var contactsGroup = app.MapGroup("/api/contacts")
    .WithTags("Kontakty");

contactsGroup.MapPost("/", (AddContactDto dto, IContactUseCases useCases) =>
    useCases.AddContact(dto).Match(
        contact => Results.Created($"/api/contacts/{contact.Id}", contact),
        error => Results.Problem(detail: error, statusCode: 400)
    ))
    .WithSummary("Dodaje kontakt")
    .WithDescription("Tworzy nowy kontakt w systemie i zwraca utworzony obiekt.")
    .Produces<AddContactResult>(StatusCodes.Status201Created)
    .Produces(StatusCodes.Status400BadRequest);

contactsGroup.MapPut("/{id:guid}", (Guid id, UpdateContactDto dto, IContactUseCases useCases) =>
    useCases.UpdateContact(id, dto).Match(
        _ => Results.NoContent(),
        error => Results.Problem(detail: error, statusCode: 400)
    ))
    .WithSummary("Aktualizuje kontakt")
    .WithDescription("Modyfikuje podstawowe dane istniejącego kontaktu.")
    .Produces(StatusCodes.Status204NoContent)
    .Produces(StatusCodes.Status400BadRequest);

contactsGroup.MapDelete("/{id:guid}", (Guid id, IContactUseCases useCases) =>
    useCases.RemoveContact(id).Match(
        _ => Results.NoContent(),
        error => Results.Problem(detail: error, statusCode: 400)
    ))
    .WithSummary("Usuwa kontakt")
    .WithDescription("Usuwa kontakt o podanym identyfikatorze z bazy danych.")
    .Produces(StatusCodes.Status204NoContent)
    .Produces(StatusCodes.Status400BadRequest);

contactsGroup.MapPost("/emails", (AddEmailDto dto, IContactUseCases useCases) =>
    useCases.AddEmail(dto).Match(
        _ => Results.NoContent(),
        error => Results.Problem(detail: error, statusCode: 400)
    ))
    .WithSummary("Dodaje adres e-mail do kontaktu")
    .WithDescription("Przypisuje nowy adres e-mail do wybranego kontaktu.")
    .Produces(StatusCodes.Status204NoContent)
    .Produces(StatusCodes.Status400BadRequest);

contactsGroup.MapDelete("/emails", ([Microsoft.AspNetCore.Mvc.FromBody] RemoveEmailDto dto, IContactUseCases useCases) =>
    useCases.RemoveEmail(dto).Match(
        _ => Results.NoContent(),
        error => Results.Problem(detail: error, statusCode: 400)
    ))
    .WithSummary("Usuwa adres e-mail z kontaktu")
    .WithDescription("Usuwa wskazany adres e-mail przypisany do kontaktu.")
    .Produces(StatusCodes.Status204NoContent)
    .Produces(StatusCodes.Status400BadRequest);

contactsGroup.MapPost("/phones", (AddPhoneDto dto, IContactUseCases useCases) =>
    useCases.AddPhone(dto).Match(
        _ => Results.NoContent(),
        error => Results.Problem(detail: error, statusCode: 400)
    ))
    .WithSummary("Dodaje numer telefonu do kontaktu")
    .WithDescription("Przypisuje nowy numer telefonu do wybranego kontaktu.")
    .Produces(StatusCodes.Status204NoContent)
    .Produces(StatusCodes.Status400BadRequest);

contactsGroup.MapDelete("/phones", ([Microsoft.AspNetCore.Mvc.FromBody] RemovePhoneDto dto, IContactUseCases useCases) =>
    useCases.RemovePhone(dto).Match(
        _ => Results.NoContent(),
        error => Results.Problem(detail: error, statusCode: 400)
    ))
    .WithSummary("Usuwa numer telefonu z kontaktu")
    .WithDescription("Usuwa wskazany numer telefonu przypisany do kontaktu.")
    .Produces(StatusCodes.Status204NoContent)
    .Produces(StatusCodes.Status400BadRequest);

contactsGroup.MapPost("/important-dates", (AddImportantDateDto dto, IContactUseCases useCases) =>
    useCases.AddImportantDate(dto).Match(
        _ => Results.NoContent(),
        error => Results.Problem(detail: error, statusCode: 400)
    ))
    .WithSummary("Dodaje ważną datę do kontaktu")
    .WithDescription("Przypisuje nową ważną datę do wybranego kontaktu.")
    .Produces(StatusCodes.Status204NoContent)
    .Produces(StatusCodes.Status400BadRequest);

contactsGroup.MapDelete("/important-dates", ([Microsoft.AspNetCore.Mvc.FromBody] RemoveImportantDateDto dto, IContactUseCases useCases) =>
    useCases.RemoveImportantDate(dto).Match(
        _ => Results.NoContent(),
        error => Results.Problem(detail: error, statusCode: 400)
    ))
    .WithSummary("Usuwa ważną datę z kontaktu")
    .WithDescription("Usuwa wskazaną ważną datę przypisaną do kontaktu.")
    .Produces(StatusCodes.Status204NoContent)
    .Produces(StatusCodes.Status400BadRequest);

app.Run();
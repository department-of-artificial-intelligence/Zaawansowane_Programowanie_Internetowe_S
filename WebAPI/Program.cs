using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Data;
using Application.UseCases.Dtos;
using Application.DomainServices;
using Application.OutputPorts;
using Application.Queries;
using Application.Repositories;
using Application.Services;
using Application.UseCases;

// Przygotowanie konfiguratora naszej aplikacji
var builder = WebApplication.CreateBuilder(args);

// Dodanie kontekstu danych
builder.Services.AddDbContext<AppDbContext>(options =>
options.UseSqlite(builder.Configuration.GetConnectionString("default"))
);

builder.Services.AddScoped<IContactRepository, ContactRepository>();
builder.Services.AddScoped<ICategoryQueries, CategoryQueries>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<ICategoryUseCases, CategoryServices>();
builder.Services.AddScoped<CategoryUniquenessChecker>();
builder.Services.AddScoped<IContactQueries, ContactQueries>();
builder.Services.AddScoped<IContactUseCases, ContactServices>();


// Dodanie usług koniecznych dla OpenApi
builder.Services.AddOpenApi();
builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<WebAPI.GlobalExceptionHandler>();



// Zbudowanie aplikacji na podstawie konfiguracji
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/openapi/v1.json", "API v1");
    });
}


app.UseExceptionHandler();

// Mapowanie endpoint OpenApi (tylko w przypadku trybu deweloperskiego).
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

// Uruchomienie przekierowania żądań z HTTP do HTTPS
app.UseHttpsRedirection();

var categoriesGroup = app.MapGroup("/api/categories")
                         .WithTags("Kategorie");

categoriesGroup.MapPost("/"
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

categoriesGroup.MapGet("/",
(ICategoryQueries queries) => Results.Ok(queries.GetCategories())
)
.WithSummary("Pobiera wszystkei kategorie")
.WithDescription("Zwraca wszystkie kateghorie które znajdują się w bazie")
.Produces(StatusCodes.Status200OK)
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
.Produces<AddCategoryResult>(StatusCodes.Status201Created)
.Produces(StatusCodes.Status400BadRequest);


app.MapGet("/api/contacts",
(IContactQueries contacts) => Results.Ok(contacts.GetContacts())
)
.WithSummary("Pobiera wszystkie kontakty")
.WithDescription("Zwraca wszystkie kontakty które znajdują się w bazie")
.WithTags("Kontakty")
.Produces(StatusCodes.Status200OK)
.Produces(StatusCodes.Status400BadRequest);


app.MapPost("/api/contacts/emails",
    (AddEmailDto dto, IContactUseCases useCases) =>
    useCases
        .AddEmail(dto)
        .Match(
            _ => Results.Ok(),
            error => Results.Problem(detail: error, statusCode: 400)
        )
    )
.WithSummary("Dodaje e-mail do kontaktu")
.WithDescription("Dodaje nowy adres e-mail dla wskazanego kontaktu i kategorii.")
.WithTags("Kontakty")
.Produces(StatusCodes.Status200OK)
.Produces(StatusCodes.Status400BadRequest);


app.MapDelete("/api/contacts/emails",
    ([FromBody] RemoveEmailDto dto, IContactUseCases useCases) =>
    useCases
        .RemoveEmail(dto)
        .Match(
            _ => Results.Ok(),
            error => Results.Problem(detail: error, statusCode: 400)
        )
    )
.WithSummary("Usuwa e-mail z kontaktu")
.WithTags("Kontakty")
.Produces(StatusCodes.Status200OK)
.Produces(StatusCodes.Status400BadRequest);

/*
app.MapPost("/api/contacts/phones",
    (AddPhoneDto dto, IContactUseCases useCases) =>
    useCases
        .AddPhone(dto)
        .Match(
            _ => Results.Ok(),
            error => Results.Problem(detail: error, statusCode: 400)
        )
    )
.WithSummary("Dodaje numer telefonu do kontaktu")
.WithTags("Kontakty")
.Produces(StatusCodes.Status200OK)
.Produces(StatusCodes.Status400BadRequest);


app.MapPut("/api/contacts",
    (UpdateContactDto dto, IContactUseCases useCases) =>
    useCases
        .UpdateContact(dto)
        .Match(
            _ => Results.Ok(),
            error => Results.Problem(detail: error, statusCode: 400)
        )
    )
.WithSummary("Aktualizuje podstawowe dane kontaktu")
.WithTags("Kontakty")
.Produces(StatusCodes.Status200OK)
.Produces(StatusCodes.Status400BadRequest);


app.MapDelete("/api/contacts/{id:guid}",
    (Guid id, IContactUseCases useCases) =>
    useCases
        .RemoveContact(id)
        .Match(
            _ => Results.Ok(),
            error => Results.Problem(detail: error, statusCode: 400)
        )
    )
.WithSummary("Usuwa kontakt o podanym ID")
.WithTags("Kontakty")
.Produces(StatusCodes.Status200OK)
.Produces(StatusCodes.Status400BadRequest);
*/


app.MapPost("/api/contacts/important-dates",
    (AddImportantDateDto dto, IContactUseCases useCases) =>
    useCases
        .AddImportantDate(dto)
        .Match(
            _ => Results.Ok(),
            error => Results.Problem(detail: error, statusCode: 400)
        )
    )
.WithSummary("Dodaje ważną datę do kontaktu")
.WithTags("Kontakty")
.Produces(StatusCodes.Status200OK)
.Produces(StatusCodes.Status400BadRequest);


app.MapDelete("/api/contacts/important-dates",
    ([FromBody] RemoveImportantDateDto dto, IContactUseCases useCases) =>
    useCases
        .RemoveImportantDate(dto)
        .Match(
            _ => Results.Ok(),
            error => Results.Problem(detail: error, statusCode: 400)
        )
    )
.WithSummary("Usuwa ważną datę z kontaktu")
.WithTags("Kontakty")
.Produces(StatusCodes.Status200OK)
.Produces(StatusCodes.Status400BadRequest);

// Uruchomienie aplikacji
app.Run();
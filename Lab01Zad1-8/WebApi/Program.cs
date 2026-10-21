using Microsoft.EntityFrameworkCore;
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

app.MapPost("/api/categories",
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
.WithTags("Kategorie")
.Produces<AddCategoryResult>(StatusCodes.Status201Created)
.Produces(StatusCodes.Status400BadRequest);

app.MapGet("/api/categories",
(ICategoryQueries queries) => Results.Ok(queries.GetCategories())
)
.WithSummary("Pobiera wszystkei kategorie")
.WithDescription("Zwraca wszystkie kateghorie które znajdują się w bazie")
.WithTags("Kategorie");


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
.WithTags("Kontakty");


// Uruchomienie aplikacji
app.Run();
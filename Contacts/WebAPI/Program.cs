using Microsoft.EntityFrameworkCore;

using Application.DomainServices;
using Application.OutputPorts; 
using Application.Queries;
using Application.Queries.Dtos;
using Application.Repositories; 
using Application.Services; 
using Application.UseCases;
using Application.UseCases.Dtos;
using Microsoft.AspNetCore.Mvc;

using Data;

var builder = WebApplication.CreateBuilder(args);
 builder.Services.AddDbContext<AppDbContext>(options =>
     options.UseSqlite(builder.Configuration.GetConnectionString("default"))
);
// Add services to the container.
builder.Services.AddScoped<ICategoryQueries, CategoryQueries>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<ICategoryUseCases, CategoryServices>();
builder.Services.AddScoped<CategoryUniquenessChecker>();
builder.Services.AddScoped<IContactRepository, ContactRepository>();
builder.Services.AddScoped<IContactUseCases, ContactServices>();
builder.Services.AddProblemDetails(); 
builder.Services.AddExceptionHandler<WebAPI.GlobalExceptionHandler>();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();
app.UseExceptionHandler();
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUI(options =>
     {
         options.SwaggerEndpoint("/openapi/v1.json", "API v1");
     });
}

app.UseHttpsRedirection();

var categoriesGroup = app.MapGroup("/api/categories")
    .WithTags("Kategorie");

 categoriesGroup.MapPost("/api/categories", (AddCategoryDto dto, ICategoryUseCases useCases) => 
    useCases
        .AddCategory(dto)
        .Match(
            category => Results.Created($"/api/categories/{category.Id}", category),
            error => Results.Problem(detail: error, statusCode: 400)
        )
)
.WithSummary("Dodaje kategorię")
.WithDescription("Tworzy nową kategorię i zwraca utworzony obiekt.")
.WithTags("Kategorie")
.Produces<AddCategoryResult>(StatusCodes.Status201Created) 
.Produces(StatusCodes.Status400BadRequest);

categoriesGroup.MapGet("/api/categories",
    (ICategoryQueries queries) => Results.Ok(queries.GetCategories())
)
.WithSummary("Pobiera kategorię")
.WithDescription("Zwraca listę kategorii")
.WithTags("Kategorie")
.Produces<IEnumerable<CategoryDto>>(StatusCodes.Status200OK) 
.Produces(StatusCodes.Status400BadRequest);

categoriesGroup.MapDelete("/api/categories/{id:int}", (Guid id, ICategoryUseCases useCases) => 
    useCases
        .RemoveCategory(id)
        .Match(
            _ => Results.NoContent(),
            error => Results.Problem(detail: error, statusCode: 400)
        )
)
.WithSummary("Usuwa kategorię")
.WithDescription("Usuwa wskazaną kategorię na podstawie przekazanego indeksa.")
.WithTags("Kategorie")
.Produces(StatusCodes.Status204NoContent) 
.Produces(StatusCodes.Status400BadRequest);

categoriesGroup.MapGet("/api/categories/{id:Guid}", (Guid id, ICategoryQueries queries) =>
    queries.GetCategory(id) is { } category
        ? Results.Ok(category)
        : Results.NotFound()
)
.WithSummary("Pobiera kategorię po id")
.WithDescription("Zwraca szczegóły kategorii na podstawie jej indeksa.")
.WithTags("Kategorie")
.Produces<CategoryDto>(StatusCodes.Status200OK)
.Produces(StatusCodes.Status404NotFound);

categoriesGroup.MapPut("/api/categories", (UpdateCategoryDto dto, ICategoryUseCases useCases) => 
    useCases
        .UpdateCategory(dto)
        .Match(
            _ => Results.NoContent(),
            error => Results.Problem(detail: error, statusCode: 400)
        )
)
.WithSummary("Aktualizuje kategorię")
.WithDescription("Aktualizuje kategorię na podstawie przesłanych danych.")
.WithTags("Kategorie")
.Produces(StatusCodes.Status204NoContent) 
.Produces(StatusCodes.Status400BadRequest);

var contactsGroup = app.MapGroup("/api/contacts")
    .WithTags("Kontakty");

contactsGroup.MapPost("/", (AddContactDto dto, IContactUseCases useCases) =>
    useCases.AddContact(dto).Match(
        result => Results.Created($"/api/contacts/{result.Id}", result),
        error => Results.Problem(detail: error, statusCode: 400)
    )
)
.WithSummary("Dodaje kontakt")
.WithDescription("Tworzy nowy kontakt w systemie.")
.Produces<AddContactResult>(StatusCodes.Status201Created)
.Produces(StatusCodes.Status400BadRequest);

contactsGroup.MapPut("/", (UpdateContactDto dto, IContactUseCases useCases) =>
    useCases.UpdateContact(dto).Match(
        _ => Results.NoContent(),
        error => Results.Problem(detail: error, statusCode: 400)
    )
)
.WithSummary("Aktualizuje kontakt")
.WithDescription("Aktualizuje dane istniejącego kontaktu.")
.Produces(StatusCodes.Status204NoContent)
.Produces(StatusCodes.Status400BadRequest);

contactsGroup.MapDelete("/{id:Guid}", (Guid id, IContactUseCases useCases) =>
    useCases.RemoveContact(id).Match(
        _ => Results.NoContent(),
        error => Results.Problem(detail: error, statusCode: 400)
    )
)
.WithSummary("Usuwa kontakt")
.WithDescription("Usuwa kontakt na podstawie jego indeksu.")
.Produces(StatusCodes.Status204NoContent)
.Produces(StatusCodes.Status400BadRequest);

contactsGroup.MapPost("/emails", (AddEmailDto dto, IContactUseCases useCases) =>
    useCases.AddEmail(dto).Match(
        _ => Results.NoContent(),
        error => Results.Problem(detail: error, statusCode: 400)
    )
)
.WithSummary("Dodaje e-mail do kontaktu")
.WithTags("Kontakty - Powiązania")
.Produces(StatusCodes.Status204NoContent)
.Produces(StatusCodes.Status400BadRequest);

contactsGroup.MapDelete("/emails", ([FromBody]RemoveEmailDto dto, IContactUseCases useCases) =>
    useCases.RemoveEmail(dto).Match(
        _ => Results.NoContent(),
        error => Results.Problem(detail: error, statusCode: 400)
    )
)
.WithSummary("Usuwa e-mail z kontaktu")
.WithTags("Kontakty - Powiązania")
.Produces(StatusCodes.Status204NoContent)
.Produces(StatusCodes.Status400BadRequest);

contactsGroup.MapPost("/phones", (AddPhoneDto dto, IContactUseCases useCases) =>
    useCases.AddPhone(dto).Match(
        _ => Results.NoContent(),
        error => Results.Problem(detail: error, statusCode: 400)
    )
)
.WithSummary("Dodaje numer telefonu do kontaktu")
.WithTags("Kontakty - Powiązania")
.Produces(StatusCodes.Status204NoContent)
.Produces(StatusCodes.Status400BadRequest);

contactsGroup.MapDelete("/phones", ([FromBody]RemovePhoneDto dto, IContactUseCases useCases) =>
    useCases.RemovePhone(dto).Match(
        _ => Results.NoContent(),
        error => Results.Problem(detail: error, statusCode: 400)
    )
)
.WithSummary("Usuwa numer telefonu z kontaktu")
.WithTags("Kontakty - Powiązania")
.Produces(StatusCodes.Status204NoContent)
.Produces(StatusCodes.Status400BadRequest);

contactsGroup.MapPost("/important-dates", (AddImportantDateDto dto, IContactUseCases useCases) =>
    useCases.AddImportantDate(dto).Match(
        _ => Results.NoContent(),
        error => Results.Problem(detail: error, statusCode: 400)
    )
)
.WithSummary("Dodaje ważną datę do kontaktu")
.WithTags("Kontakty - Powiązania")
.Produces(StatusCodes.Status204NoContent)
.Produces(StatusCodes.Status400BadRequest);

contactsGroup.MapDelete("/important-dates", ([FromBody]RemoveImportantDateDto dto, IContactUseCases useCases) =>
    useCases.RemoveImportantDate(dto).Match(
        _ => Results.NoContent(),
        error => Results.Problem(detail: error, statusCode: 400)
    )
)
.WithSummary("Usuwa ważną datę z kontaktu")
.WithTags("Kontakty - Powiązania")
.Produces(StatusCodes.Status204NoContent)
.Produces(StatusCodes.Status400BadRequest);

contactsGroup.MapGet("/", (IContactRepository repository) =>
    Results.Ok(repository.GetAll()) 
)
.WithSummary("Pobiera wszystkie kontakty")
.WithDescription("Zwraca listę wszystkich kontaktów zapisanych w systemie.")
.Produces(StatusCodes.Status200OK); 

contactsGroup.MapGet("/{id:Guid}", (Guid id, IContactRepository repository) =>
    repository.GetById(id) is { } contact
        ? Results.Ok(contact)
        : Results.NotFound()
)
.WithSummary("Pobiera kontakt po Id")
.WithDescription("Zwraca szczegóły konkretnego kontaktu na podstawie jego indeksu.")
.Produces(StatusCodes.Status200OK)
.Produces(StatusCodes.Status404NotFound);
app.Run();


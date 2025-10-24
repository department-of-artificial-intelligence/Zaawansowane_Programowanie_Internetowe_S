using Contacts.Application;
using Contacts.Application.Queries;
using Contacts.Application.Repositories;
using Contacts.Application.Services;
using Contacts.Application.UseCases;
using Contacts.Application.UseCases.DTOs;
using Contacts.Data;
using Microsoft.EntityFrameworkCore;

// Przygotowanie konfiguratora naszej aplikacji
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("default"))
);

builder.Services.AddTransient<ICategoryQueries, CategoriesQueries>();
builder.Services.AddTransient<IUnitOfWork, UnitOfWork>();
builder.Services.AddTransient<ICategoryRepository, CategoryRepository>();
builder.Services.AddTransient<ICategoryUseCases, CategoryServices>();
builder.Services.AddTransient<IContactQueries, ContactQueries>();
builder.Services.AddTransient<IContactRepository, ContactRepository>();
builder.Services.AddTransient<IContactUseCases, ContactServices>();

// Dodanie us!ug koniecznych dla Swagger’a
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Zbudowanie aplikacji na podstawie konfiguracji
var app = builder.Build();

// Uruchomienie swagger’a (tylko w przypadku trybu deweloperskiego).
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Uruchomienie przekierowania "#da$ z HTTP do HTTPS
app.UseHttpsRedirection();

app.MapPost(
        "/api/categories",
        (AddCategoryDTO dto, ICategoryUseCases useCases) =>
        {
            try
            {
                var category = useCases.AddCategory(dto);
                return Results.Created($"/categories/category.Id", category);
            }
            catch (Exception exception)
            {
                return Results.Problem(detail: exception.Message, title: "Błąd");
            }
        }
    )
    .WithOpenApi(
        (operation) =>
        {
            operation.Responses["201"].Description = "Kategoria zosta!a dodana";
            operation.Responses["500"].Description = "Wystapil błąd";
            return operation;
        }
    )
    .Produces<AddCategoryResult>(StatusCodes.Status201Created)
    .Produces(StatusCodes.Status500InternalServerError);

app.MapGet(
        "/api/categories",
        (ICategoryQueries queries) =>
        {
            try
            {
                var categories = queries.GetCategories();
                return Results.Ok(categories);
            }
            catch (Exception exception)
            {
                return Results.Problem(detail: exception.Message, title: "Błąd");
            }
        }
    )
    .WithDescription("Tworzy nowa kategorie")
    .WithSummary("Tworzy nowa kategorie")
    .WithTags("Kategoria");
;

app.MapPost(
        "/api/contacts",
        (AddContactDTO dto, IContactUseCases useCases) =>
        {
            try
            {
                var contact = useCases.AddContact(dto);
                return Results.Created($"/contacts/contact.Id", contact);
            }
            catch (Exception exception)
            {
                return Results.Problem(detail: exception.Message, title: "Błąd");
            }
        }
    )
    .WithOpenApi(
        (operation) =>
        {
            operation.Responses["201"].Description = "Kategoria zosta!a dodana";
            operation.Responses["500"].Description = "Wystapil błąd";
            return operation;
        }
    )
    .Produces<AddContactResult>(StatusCodes.Status201Created)
    .Produces(StatusCodes.Status500InternalServerError);

// Uruchomienie aplikacji
app.Run();

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

// Dodanie kontekstu danych
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("default"))
);

builder.Services.AddTransient<ICategoryQueries, CategoriesQueries>();
builder.Services.AddTransient<IUnitOfWork, UnitOfWork>();
builder.Services.AddTransient<ICategoryRepository, CategoryRepository>();
builder.Services.AddTransient<ICategoryUseCases, CategoryServices>();

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

app.UseHttpsRedirection();
app.MapPost(
        "/api/categories",
        (AddCategoryDTO dto, ICategoryUseCases useCases) =>
        {
            try
            {
                useCases.AddCategory(dto); //?
                return Results.Created($"/categories/category.Id", dto); //?
            }
            catch
            {
                return Results.Problem(
                    detail: "Wystpil blad podczas realizacji tego zdania",
                    title: "Bladd"
                );
            }
        }
    )
    .WithOpenApi(
        (operation) =>
        {
            operation.Responses["201"].Description = "Kategoria zostala dodana";
            operation.Responses["500"].Description = "Wystapil blad";
            return operation;
        }
    )
    .WithDescription("Tworzy nowa kategorie")
    .WithSummary("Tworzy nowa kategorie")
    .WithTags("Kategoria")
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
        catch
        {
            return Results.Problem(
                detail: "Wystpil blad podczas realizacji tego zdania",
                title: "Blad"
            );
        }
    }
);

// Uruchomienie aplikacji
app.Run();

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

// Dodanie us!ug koniecznych dla Swagger’a
builder.Services.AddOpenApi();

// Zbudowanie aplikacji na podstawie konfiguracji
var app = builder.Build();

// Uruchomienie swagger’a (tylko w przypadku trybu deweloperskiego).
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
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
        catch
        {
            return Results.Problem(
                detail: "Wystpi! błąd podczas realizacji tego żądania",
                title: "Błąd"
            );
        }
    }
);

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
                detail: "Wystpi! błąd podczas realizacji tego żądania",
                title: "Błąd"
            );
        }
    }
);

// Uruchomienie aplikacji
app.Run();

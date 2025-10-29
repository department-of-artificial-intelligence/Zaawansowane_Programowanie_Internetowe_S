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

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
// Dodanie us!ug koniecznych dla Swagger’a
builder.Services.AddTransient<ICategoryQueries, CategoriesQueries>();
builder.Services.AddTransient<IUnitOfWork, UnitOfWork>();
builder.Services.AddTransient<ICategoryRepository, CategoryRepository>();
builder.Services.AddTransient<ICategoryUseCases, CategoryServices>();
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
                detail: "Wystpi! b!d podczas realizacji tego zdania",
                title: "Blad"
            );
        }
    }
);

// Uruchomienie aplikacji
app.Run();

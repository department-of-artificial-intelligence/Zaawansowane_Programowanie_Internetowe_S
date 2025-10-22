using Microsoft.EntityFrameworkCore;

using Contacts.Application;
using Contacts.Application.Queries;
using Contacts.Application.Services;
using Contacts.Application.UseCases;
using Contacts.Data;
using Contacts.Application.UseCases.DTOs;
using Contacts.Application.Repositories;

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
// Uruchomienie przekierowania "#da$ z HTTP do HTTPS 
app.UseHttpsRedirection();

app.MapPost("/api/categories", (AddCategoryDTO dto, ICategoryUseCases useCases) =>
{
    try
    {
        var category = useCases.AddCategory(dto);
        return Results.Created($"/categories/category.Id", category);
    }
    catch
    {
        return Results.Problem(
        detail: "Wystpił błądpodczas realizacji tego dania",
        title: "Błąd"
        );
    }
});

builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();

// Uruchomienie aplikacji 
app.Run();

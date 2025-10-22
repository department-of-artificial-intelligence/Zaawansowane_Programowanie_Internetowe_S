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
app.MapPost("/api/categories", (AddCategoryDTO dto, ICategoryUseCases useCases) => {try
{
var category = useCases.AddCategory(dto);
return Results.Created($"/categories/category.Id", category);
}
catch
{
return Results.Problem(
detail: "Wystpi! b!"d podczas realizacji tego #"dania",
title: "B!"d"
);
} });

// Uruchomienie aplikacji
app.Run();

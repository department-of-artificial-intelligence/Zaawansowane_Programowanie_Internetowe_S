using System.Text.Json.Serialization;
using Application.DomainServices;
using Application.OutputPorts;
using Application.Queries;
using Application.Repositories;
using Application.Services;
using Application.UseCases;
using Data;
using Microsoft.EntityFrameworkCore;
using WebAPI;
using WebAPI.Endpoints;

var builder = WebApplication.CreateBuilder(args);

// Kontekst danych
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("default")));

// Repozytoria, zapytania i jednostka pracy
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<IContactRepository, ContactRepository>();
builder.Services.AddScoped<ICategoryQueries, CategoryQueries>();
builder.Services.AddScoped<IContactQueries, ContactQueries>();

// Usługi domenowe
builder.Services.AddScoped<CategoryUniquenessChecker>();
builder.Services.AddScoped<CategoryRemovalChecker>();

// Przypadki użycia
builder.Services.AddScoped<ICategoryUseCases, CategoryServices>();
builder.Services.AddScoped<IContactUseCases, ContactServices>();

// Enumy (Sex, PhoneType) jako tekst w JSON-ie i w Swaggerze, np. "Male" zamiast 0
builder.Services.ConfigureHttpJsonOptions(options =>
    options.SerializerOptions.Converters.Add(new JsonStringEnumConverter()));

builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddOpenApi();

var app = builder.Build();

// Baza zawsze w aktualnej wersji (tabele Phone i ImportantDate z Lab1)
using (var scope = app.Services.CreateScope())
{
    scope.ServiceProvider.GetRequiredService<AppDbContext>().Database.Migrate();
}

app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUI(options =>
        options.SwaggerEndpoint("/openapi/v1.json", "API v1"));
}

app.UseHttpsRedirection();

app.MapCategoryEndpoints();
app.MapContactEndpoints();

app.Run();

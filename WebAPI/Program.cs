using Microsoft.EntityFrameworkCore;
using Contacts.Data;
using Contacts.Application.UseCases.DTOs;

// Przygotowanie konfiguratora naszej aplikacji
var builder = WebApplication.CreateBuilder(args);

// Dodanie kontekstu danych
builder.Services.AddDbContext<AppDbContext>(options =>
options.UseSqlite(builder.Configuration.GetConnectionString("default"))
);

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

app.MapPost("/api/categories", (AddCategoryDTO dto) => {
});

// Uruchomienie aplikacji
app.Run();

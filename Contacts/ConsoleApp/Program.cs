using Contacts.Application.Domain;
using Contacts.Application.Services;
using Contacts.Application.UseCases.DTOs;
using Contacts.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

// Wczytanie pliku konfiguracyjnego
var builder = new ConfigurationBuilder();
builder
    .SetBasePath(AppContext.BaseDirectory)
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

// Skonfigurowanie po!"czenia z baz" danych
IConfiguration config = builder.Build();
var dbOptionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
dbOptionsBuilder.UseSqlite(config.GetConnectionString("default"));

// Stworzenie kontekstu
using var context = new AppDbContext(dbOptionsBuilder.Options);
context.Database.Migrate();
context.Database.EnsureCreated();

// Uruchomienie aplikacji
var categoryRepository = new CategoryRepository(context);
var unitOfWork = new UnitOfWork(context);
var categoryServices = new CategoryServices(categoryRepository, unitOfWork);
//categoryServices.AddCategory(new AddCategoryDTO { Name = "nowa kategoria 3"});
//categoryServices.RemoveCategory(1);
foreach (var category in context.Categories)
{
    Console.WriteLine(category.Name);
}

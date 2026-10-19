using Contacts.Application.Services;
using Contacts.Application.UseCases.DTOs;
using Contacts.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;


var builder = new ConfigurationBuilder();
builder.SetBasePath(AppContext.BaseDirectory)
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);


IConfiguration config = builder.Build();
var dbOptionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
dbOptionsBuilder.UseSqlite(config.GetConnectionString("default"));

using var context = new AppDbContext(dbOptionsBuilder.Options);
context.Database.Migrate();

var categoryRepository = new CategoryRepository(context);
var unitOfWork = new UnitOfWork(context);
var categoryServices = new CategoryServices(categoryRepository, unitOfWork);
var categoryQueries = new CategoriesQueries(context);

try
{
    var added = categoryServices.AddCategory(new AddCategoryDTO { Name = "Praca" });
    Console.WriteLine($"Dodano: {added.Id} - {added.Name}");
    categoryServices.AddCategory(new AddCategoryDTO { Name = "Dom" });
}
catch (ServiceException ex)
{
    Console.WriteLine($"Błąd: {ex.Message}");
}

Console.WriteLine("Kategorie:");
foreach (var category in categoryQueries.GetCategories())
{
    Console.WriteLine($"{category.Id}: {category.Name}");
}

try
{
    categoryServices.RemoveCategory(1);
    Console.WriteLine("Usunięto kategorię 1");
}
catch (ServiceException ex)
{
    Console.WriteLine($"Błąd: {ex.Message}");
}
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Contacts.Application.Domain;
using Contacts.Application.Services;
using Contacts.Application.UseCases.DTOs;
using Contacts.Data;


var builder = new ConfigurationBuilder();
builder.SetBasePath(AppContext.BaseDirectory)
        .AddJjonFile(
            "appsetting.json",
            optional: false,
            reloadOnChange: true
        );

IConfiguration config = builder.Build();
var dbOptionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
dbOptionsBuilder.UseSqlite(config.GetConnectionString("default"));

using var context = new AppDbContext(dbOptionsBuilder.Options);
context.Database.Migrate();
context.Database.EnsureCreated();

var categoryRepository = new CategoryRepository(context);
var unitOfWork = new UnitOfWork(context);
var categoryServices = new CategoryServices(categoryRepository, unitOfWork);

categoryServices.RemoveCategory(1);

foreach(var category in context.Categories)
{
    Console.WriteLine(category.Name);
}
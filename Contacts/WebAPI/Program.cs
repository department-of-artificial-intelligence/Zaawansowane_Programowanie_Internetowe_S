using Contacts.Application;
using Contacts.Application.Queries;
using Contacts.Application.Repositories;
using Contacts.Application.Services;
using Contacts.Application.UseCases;
using Contacts.Application.UseCases.DTOs;
using Contacts.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("default"))
);

builder.Services.AddTransient<ICategoryQueries, CategoriesQueries>();
builder.Services.AddTransient<IUnitOfWork, UnitOfWork>();
builder.Services.AddTransient<ICategoryRepository, CategoryRepository>();
builder.Services.AddTransient<ICategoryUseCases, CategoryServices>();

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Contacts API V1");
        c.RoutePrefix = string.Empty;
    });
}

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
                return Results.Problem(detail: "Wystpi! blad podczas realizacji tego zadania");
            }
        }
    )
    .WithOpenApi(
        (operation) =>
        {
            operation.Responses["201"].Description = "Kategoria zosta!a dodana";
            operation.Responses["500"].Description = "Wystapil blad";
            return operation;
        }
    )
    .WithDescription("Tworzy nowa kategorie")
    .WithSummary("Tworzy nowa kategorie")
    .WithTags("Kategoria")
    .Produces<AddCategoryResult>(StatusCodes.Status201Created)
    .Produces(StatusCodes.Status500InternalServerError);

app.MapPost(
        "/api/categories/edit",
        (EditCategoryDTO dto, ICategoryUseCases useCases) =>
        {
            try
            {
                var category = useCases.EditCategory(dto);
                return Results.Created($"/categories/category.Id", category);
            }
            catch
            {
                return Results.Problem(detail: "Wystapil blad podczas edycji");
            }
        }
    )
    .WithOpenApi(
        (operation) =>
        {
            operation.Responses["201"].Description = "Kategoria zostala zedytowana";
            operation.Responses["500"].Description = "Wystapil blad podczas edycji";
            return operation;
        }
    )
    .WithDescription("Aktualizuje nowa kategorie")
    .WithSummary("Aktualizuje nowa kategorie")
    .WithTags("Kategoria")
    .Produces<AddCategoryResult>(StatusCodes.Status201Created)
    .Produces(StatusCodes.Status500InternalServerError);

app.MapPost(
        "/api/categories/{:id}",
        (int id, ICategoryUseCases useCases) =>
        {
            try
            {
                useCases.RemoveCategory(id);
                return Results.Ok();
            }
            catch
            {
                return Results.Problem(detail: "Wystapil blad podczas usuwania");
            }
        }
    )
    .WithOpenApi(
        (operation) =>
        {
            operation.Responses["200"].Description = "Kategoria zostala usunieta";
            operation.Responses["500"].Description = "Wystapil blad podczas usuwania";
            return operation;
        }
    )
    .WithDescription("Aktualizuje nowa kategorie")
    .WithSummary("Aktualizuje nowa kategorie")
    .WithTags("Kategoria")
    .Produces<AddCategoryResult>(StatusCodes.Status200OK)
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
        catch (Exception ex)
        {
            return Results.Problem(detail: ex.Message, title: "Błąd serwera");
        }
    }
);

// var summaries = new[]
// {
//     "Freezing",
//     "Bracing",
//     "Chilly",
//     "Cool",
//     "Mild",
//     "Warm",
//     "Balmy",
//     "Hot",
//     "Sweltering",
//     "Scorching",
// };

// app.MapGet(
//         "/weatherforecast",
//         () =>
//         {
//             var forecast = Enumerable
//                 .Range(1, 5)
//                 .Select(index => new WeatherForecast(
//                     DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
//                     Random.Shared.Next(-20, 55),
//                     summaries[Random.Shared.Next(summaries.Length)]
//                 ))
//                 .ToArray();
//             return forecast;
//         }
//     )
//     .WithName("GetWeatherForecast");

// builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();

app.Run();

// record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
// {
//     public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
// }

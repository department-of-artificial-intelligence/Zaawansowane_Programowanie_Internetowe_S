using Microsoft.EntityFrameworkCore;
using Data;
using Application.UseCases.DTOs;
using Application.Repositories;
using Application.UseCases;
using Application.Services;
using Application;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("default"), b => b.MigrationsAssembly("WebAPI"))
);

builder.Services.AddTransient<ICategoryQueries, CategoriesQueries>();
builder.Services.AddTransient<IUnitOfWork, UnitOfWork>();
builder.Services.AddTransient<ICategoryRepository, CategoryRepository>();
builder.Services.AddTransient<CategoryServices>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//app.UseHttpsRedirection();

app.MapPost("/api/categories", (AddCategoryDTO dto, CategoryServices useCases) =>
{
    try
    {
        var category = useCases.AddCategory(dto);
        return Results.Created($"/categories/category.Id", category);
    }
    catch
    {
        return Results.Problem(
            detail: "Wystpil błąd podczas realizacji tego żądania",
            title: "Błąd"
        );
    }
})
.WithOpenApi((operation) =>
{
    operation.Responses["201"].Description = "Kategoria została dodana";
    operation.Responses["500"].Description = "Wystąpił błąd!";
    return operation;
})
.WithDescription("Tworzy nową kategorie")
.WithSummary("Tworzy nową kategorie")
.WithTags("Kategoria")
.Produces<AddCategoryResult>(StatusCodes.Status201Created)
.Produces(StatusCodes.Status500InternalServerError);

app.MapGet("/api/categories", (ICategoryQueries queries) => {
    try
    {
        var categories = queries.GetCategories();
        return Results.Ok(categories);
    }
    catch
    {
        return Results.Problem(
            detail: "Wystpil błąd podczas realizacji tego żądania",
            title: "Błąd"
        );
    }
});

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
{
    var forecast =  Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
})
.WithName("GetWeatherForecast");

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}

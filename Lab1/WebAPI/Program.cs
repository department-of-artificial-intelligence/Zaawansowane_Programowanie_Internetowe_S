using Application.DomainServices;
using Application.OutputPorts;
using Application.Queries;
using Application.Repositories;
using Application.Services;
using Application.UseCases;
using Application.UseCases.Dtos;
using Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// 1. Rejestracja bazy danych
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("default"))
);

// 2. Rejestracja zależności (DI)
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

builder.Services.AddScoped<ICategoryQueries, CategoryQueries>();
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<ICategoryUseCases, CategoryServices>();
builder.Services.AddScoped<CategoryUniquenessChecker>();

builder.Services.AddScoped<IContactRepository, ContactRepository>();
builder.Services.AddScoped<IContactUseCases, ContactServices>();

// 3. Obsługa błędów Problem Details i wyjątki
builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<WebAPI.GlobalExceptionHandler>();

// 4. OpenAPI
builder.Services.AddOpenApi();

var app = builder.Build();

// 5. Middleware HTTP
app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/openapi/v1.json", "API v1");
    });
}

//app.UseHttpsRedirection();

// 6. Endpointy
app.MapGet("/api/categories", (ICategoryQueries queries) =>
    Results.Ok(queries.GetCategories())
);

app.MapPost("/api/categories", (AddCategoryDto dto, ICategoryUseCases useCases) =>
    useCases
        .AddCategory(dto)
        .Match(
            category => Results.Created($"/api/categories/{category.Id}", category),
            error => Results.Problem(detail: error, statusCode: 400)
        )
)
.WithSummary("Dodaje kategorię")
.WithDescription("Tworzy nową kategorię i zwraca utworzony obiekt.")
.WithTags("Kategorie")
.Produces<AddCategoryResult>(StatusCodes.Status201Created)
.Produces(StatusCodes.Status400BadRequest);


app.Run();
using Microsoft.EntityFrameworkCore;

using Application.DomainServices;
using Application.OutputPorts;
using Application.Queries;
using Application.Repositories;
using Application.Services;
using Application.UseCases;
using Application.UseCases.Dtos;
using Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(options => 
	options.UseSqlite(builder.Configuration.GetConnectionString("default"))
);

builder.Services.AddScoped<ICategoryQueries, CategoryQueries>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<IContactRepository, ContactRepository>();
builder.Services.AddScoped<ICategoryUseCases, CategoryServices>();
builder.Services.AddScoped<CategoryUniquenessChecker>();

builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<WebAPI.GlobalExceptionHandler>();

builder.Services.AddOpenApi();

var app = builder.Build();

if(app.Environment.IsDevelopment()){
	app.MapOpenApi();

	app.UseSwaggerUI(options => {
		options.SwaggerEndpoint("/openapi/v1.json", "API v1");
	});
}

app.UseExceptionHandler();
app.UseHttpsRedirection();

app.MapGet("/api/categories", (ICategoryQueries queries) => Results.Ok(queries.GetCategories()));

app.MapPost("/api/categories", (AddCategoryDto dto, ICategoryUseCases useCases) => 
	useCases.AddCategory(dto).Match(
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

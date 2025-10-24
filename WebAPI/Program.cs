using Microsoft.EntityFrameworkCore;
using Data;
using Application.UseCases.DTOs;
using Application.Repositories;
using Application.UseCases;
using Application.Services;
using Application;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Application.DomainServices;
using Application.Helpers;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("default"), b => b.MigrationsAssembly("WebAPI"))
);

builder.Services.AddTransient<ICategoryQueries, CategoriesQueries>();
builder.Services.AddTransient<IUnitOfWork, UnitOfWork>();
builder.Services.AddTransient<ICategoryRepository, CategoryRepository>();
builder.Services.AddTransient<CategoryServices>();

builder.Services.AddTransient<IUserRepository, UserRepository>();
builder.Services.AddTransient<IUserUseCases, UserServices>();

builder.Services.AddSingleton<ITokenGenerator, TokenGenerator>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(option =>
{
    option.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please enter a valid token",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Scheme = "Bearer"
    });
    option.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type=ReferenceType.SecurityScheme,
                    Id="Bearer"
                }
            },
            new string[]{}
        }
    });
});

builder.Services.AddAuthentication()
.AddJwtBearer(x =>
{
    x.TokenValidationParameters = new TokenValidationParameters
    {
        IssuerSigningKey = new SymmetricSecurityKey(
        Encoding.UTF8
            .GetBytes(builder.Configuration["ApplicationSettings:JWT_KEY"])
        ),
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateLifetime = true
    };
});

builder.Services.AddAuthorization();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//app.UseHttpsRedirection();
app.UseAuthorization();

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
.Produces(StatusCodes.Status500InternalServerError)
.RequireAuthorization();

app.MapGet("/api/categories", (ICategoryQueries queries) =>
{
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

app.MapPost("/api/users", (AddUserDTO dto, IUserUseCases useCases) => {
    try
    {
        var user = useCases.AddUser(dto);
        return Results.Created($"/api/users/{user.Id}", user);
    }
    catch
    {
        return Results.Problem(
            detail: "Wystąpił błąd podczas realizacji tego żądania",
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
    var forecast = Enumerable.Range(1, 5).Select(index =>
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

app.MapPost("/api/login", (LoginUserDTO dto, IUserUseCases userUseCases) =>
{
    try
    {
        var loginResult = userUseCases.LoginUser(dto);
        if (loginResult.Status == UserLoginStatus.UserLogged)
        {
            return Results.Ok(new { AccessToken = loginResult.Token });
        }
        else
        {
            return Results.Unauthorized();
        }
    }
    catch
    {
        return Results.Problem(detail: "Wystąpił błąd podczas realizacji tego żądania", title: "Błąd");
    }
});

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}

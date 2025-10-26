using System.Security.Claims;
using System.Text;
using Contacts.Application;
using Contacts.Application.DomainServices;
using Contacts.Application.Queries;
using Contacts.Application.Repositories;
using Contacts.Application.Services;
using Contacts.Application.UseCases;
using Contacts.Application.UseCases.DTOs;
using Contacts.Data;
using Contacts.WebAPI.Helpers;
using Contacts.WebAPI.Inputs;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Contacts.WebAPI.Inputs;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(option =>
{
    option.AddSecurityDefinition(
        "Bearer",
        new OpenApiSecurityScheme
        {
            In = ParameterLocation.Header,
            Description = "Please enter a valid token",
            Name = "Authorization",
            Type = SecuritySchemeType.Http,
            BearerFormat = "JWT",
            Scheme = "Bearer",
        }
    );
    option.AddSecurityRequirement(
        new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer",
                    },
                },
                new string[] { }
            },
        }
    );
});

builder
    .Services.AddAuthentication()
    .AddJwtBearer(x =>
    {
        x.TokenValidationParameters = new TokenValidationParameters
        {
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["ApplicationSettings:JWT_KEY"])
            ),
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
        };
    });
builder.Services.AddAuthorization();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("default"))
);

builder.Services.AddTransient<ICategoryQueries, CategoriesQueries>();
builder.Services.AddTransient<IUnitOfWork, UnitOfWork>();
builder.Services.AddTransient<ICategoryRepository, CategoryRepository>();
builder.Services.AddTransient<ICategoryUseCases, CategoryServices>();
builder.Services.AddTransient<IContactQueries, ContactQueries>();
builder.Services.AddTransient<IContactRepository, ContactRepository>();
builder.Services.AddTransient<IContactUseCases, ContactServices>();
builder.Services.AddTransient<IUserRepository, UserRepository>();
builder.Services.AddTransient<IUserUseCases, UserServices>();
builder.Services.AddSingleton<ITokenGenerator, TokenGenerator>();

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    try
    {
        var context = services.GetRequiredService<AppDbContext>();
        context.Database.Migrate();
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Błąd podczas migracji bazy danych");
    }
}

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

    Console.WriteLine("EF Core używa pliku bazy:");
    Console.WriteLine(context.Database.GetConnectionString());

    var pending = context.Database.GetPendingMigrations();
    Console.WriteLine("Pending migrations:");
    foreach (var m in pending)
        Console.WriteLine(m);
}

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
app.UseAuthorization();

app.MapPost(
    "/api/users",
    (AddUserDTO dto, IUserUseCases useCases) =>
    {
        try
        {
            var user = useCases.AddUser(dto);
            return Results.Created($"/api/users/{user.Id}", user);
        }
        catch
        {
            return Results.Problem(detail: "Blad usera", title: "Blad");
        }
    }
);

app.MapPost(
    "/api/login",
    (LoginUserDTO dto, IUserUseCases userUseCases) =>
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
        catch (Exception e)
        {
            return Results.Problem(detail: e.Message, title: "Błąd");
        }
    }
);

// KATEGORIE
app.MapPost(
        "/api/categories",
        (AddCategory addCategory, ICategoryUseCases useCases, HttpContext httpContext) =>
        {
            try
            {
                var user = httpContext.User;
                var userId = int.Parse(user.FindFirst(ClaimTypes.NameIdentifier)?.Value);
                var category = useCases.AddCategory(
                    new AddCategoryDTO { Name = addCategory.Name, UserId = userId }
                );
                return Results.Created($"/api/categories/{category.Id}", category);
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
    .Produces(StatusCodes.Status500InternalServerError)
    .RequireAuthorization();

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
        "/api/categories/{id}",
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
    .WithDescription("Usuwa nowa kategorie")
    .WithSummary("Usuwa nowa kategorie")
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
    )
    .WithDescription("Wyświetla wszystkie")
    .WithSummary("Wyświetla wszystkie kategorie")
    .WithTags("Kategoria");

app.MapGet(
        "/api/categories/{id}",
        (int id, ICategoryQueries queries) =>
        {
            try
            {
                var category = queries.GetCategory(id);
                return Results.Ok(category);
            }
            catch (Exception ex)
            {
                return Results.Problem(detail: ex.Message, title: "Błąd serwera");
            }
        }
    )
    .WithDescription("Wyświetla wybraną kategorie po id")
    .WithSummary("Wyświetla wybraną kategorie po id")
    .WithTags("Kategoria");

// CONTACTS
app.MapPost(
        "/api/contacts",
        (AddContactDTO dto, IContactUseCases useCases) =>
        {
            try
            {
                var contact = useCases.AddContact(dto);
                return Results.Created($"/contacts/contact.Id", contact);
            }
            catch
            {
                return Results.Problem(detail: "Blad podczas dodawania kontaktu");
            }
        }
    )
    .WithDescription("Dodaje nowy kontakt")
    .WithSummary("Dodaje nowy kontakt")
    .WithTags("Kontakt");

app.MapPost(
        "/api/contacts/edit",
        (EditContactDTO dto, IContactUseCases useCases) =>
        {
            try
            {
                var contact = useCases.EditContact(dto);
                return Results.Created($"/contacts/contact.Id", contact);
            }
            catch
            {
                return Results.Problem(detail: "Blad podczas edycji kontaktu");
            }
        }
    )
    .WithDescription("Aktualizuje kontakt")
    .WithSummary("Aktualizuje kontakt")
    .WithTags("Kontakt");

app.MapPost(
        "/api/contacts/{id}",
        (int id, IContactUseCases useCases) =>
        {
            try
            {
                useCases.RemoveContact(id);
                return Results.Ok();
            }
            catch
            {
                return Results.Problem(detail: "Wystapil blad podczas usuwania");
            }
        }
    )
    .WithDescription("Usuwa kontakt")
    .WithSummary("Usuwa kontakt")
    .WithTags("Kontakt");

app.MapGet(
        "/api/contacts",
        (IContactQueries queries) =>
        {
            try
            {
                var contacts = queries.GetContacts();
                return Results.Ok(contacts);
            }
            catch (Exception ex)
            {
                return Results.Problem(detail: ex.Message, title: "Błąd serwera");
            }
        }
    )
    .WithDescription("Wyświetla wszystkie kontakty")
    .WithSummary("Wyświetla wszystkie kontakty")
    .WithTags("Kontakt");

app.MapGet(
        "/api/contacts/{id}",
        (int id, IContactQueries queries) =>
        {
            try
            {
                var contact = queries.GetContact(id);
                return Results.Ok(contact);
            }
            catch (Exception ex)
            {
                return Results.Problem(detail: ex.Message, title: "Błąd serwera");
            }
        }
    )
    .WithDescription("Wyświetla wybrany kontakt po id")
    .WithSummary("Wyświetla wybrany kontakt po id")
    .WithTags("Kontakt");

app.Run();

// record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
// {
//     public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
// }

using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using System.Text;
using Data;
using Application.UseCases.Dtos;
using Application.DomainServices;
using Application.OutputPorts;
using Application.Queries;
using Application.Repositories;
using Application.Services;
using Application.UseCases;
using Application.OutputPorts;
using WebAPI.Helpers;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using WebAPI.Inputs;


// Przygotowanie konfiguratora naszej aplikacji
var builder = WebApplication.CreateBuilder(args);

// Dodanie kontekstu danych
builder.Services.AddDbContext<AppDbContext>(options =>
options.UseSqlite(builder.Configuration.GetConnectionString("default"))
);

builder.Services.AddScoped<IContactRepository, ContactRepository>();
builder.Services.AddScoped<ICategoryQueries, CategoryQueries>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<ICategoryUseCases, CategoryServices>();
builder.Services.AddScoped<CategoryUniquenessChecker>();
builder.Services.AddScoped<IContactQueries, ContactQueries>();
builder.Services.AddScoped<IContactUseCases, ContactServices>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUserUseCases, UserServices>();
builder.Services.AddScoped<ITokenGenerator, TokenGenerator>();




// Dodanie usług koniecznych dla OpenApi
builder.Services.AddOpenApi(options =>
{
    options.AddDocumentTransformer((document, context, ct) =>
    {
        document.Components ??= new OpenApiComponents();
        document.Components.SecuritySchemes ??=
            new Dictionary<string, IOpenApiSecurityScheme>();
        document.Components.SecuritySchemes["Bearer"] =
            new OpenApiSecurityScheme
        {
            Type = SecuritySchemeType.Http,
            Scheme = "bearer",
            BearerFormat = "JWT",
            Description = "Wklej token JWT otrzymany z /api/login"
        };
            document.Security =
        [
            new OpenApiSecurityRequirement
            {
                [new OpenApiSecuritySchemeReference("Bearer", document)] = []
            }
        ];
        return Task.CompletedTask;
    });
});

builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<WebAPI.GlobalExceptionHandler>();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
.AddJwtBearer(x => {
    x.TokenValidationParameters = new TokenValidationParameters {
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8
            .GetBytes(builder.Configuration["ApplicationSettings:JWT_KEY"]!)
        ),
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateLifetime = true,
        NameClaimType = "name",
        RoleClaimType = "role"
    };
    x.MapInboundClaims = false;
    });
builder.Services.AddAuthorization();



// Zbudowanie aplikacji na podstawie konfiguracji
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/openapi/v1.json", "API v1");
    });
}


app.UseExceptionHandler();

// Mapowanie endpoint OpenApi (tylko w przypadku trybu deweloperskiego).
if (app.Environment.IsDevelopment())
{
app.MapOpenApi();
}

// Uruchomienie przekierowania żądań z HTTP do HTTPS
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();


var categoriesGroup = app.MapGroup("/api/categories")
                         .WithTags("Kategorie");

app.MapPost("/api/categories",
    (AddCategoryRequest input,
    ICategoryUseCases useCases,
    ClaimsPrincipal user) =>
        useCases
            .AddCategory(new AddCategoryDto(
                input.Name,
                Guid.Parse(user.FindFirstValue(JwtRegisteredClaimNames.Sub)!))
            )
            .Match(
                category =>
                    Results.Created(
                        $"/api/categories/{category.Id}",
                        category),
                error => Results.BadRequest(error)
            )
)
.RequireAuthorization()
.WithTags("Kategorie")
.WithSummary("Dodaje kategorię")
.WithDescription("Tworzy nową kategorię i zwraca informacje o niej");

categoriesGroup.MapGet("/",
(ICategoryQueries queries, ClaimsPrincipal user) =>
    Results.Ok(queries.GetCategories(
       Guid.Parse(user.FindFirstValue(JwtRegisteredClaimNames.Sub)!)))
)
.RequireAuthorization()
.WithSummary("Pobiera wszystkei kategorie")
.WithDescription("Zwraca wszystkie kateghorie które znajdują się w bazie")
.Produces(StatusCodes.Status200OK)
.Produces(StatusCodes.Status400BadRequest);


app.MapPost("/api/contacts",
    (AddContactDto dto, IContactUseCases useCases) =>
    useCases
        .AddContact(dto)
        .Match(
            contact => Results.Created(
                $"/api/contacts/{contact.Id}",
                contact),
            error => Results.Problem(detail: error, statusCode: 400)
        )
    )
.WithSummary("Dodaje kontakt")
.WithDescription("Tworzy nowy kontakt i zwraca utworzony obiekt.")
.WithTags("Kontakty")
.Produces<AddCategoryResult>(StatusCodes.Status201Created)
.Produces(StatusCodes.Status400BadRequest);


app.MapGet("/api/contacts",
(IContactQueries contacts) => Results.Ok(contacts.GetContacts())
)
.WithSummary("Pobiera wszystkie kontakty")
.WithDescription("Zwraca wszystkie kontakty które znajdują się w bazie")
.WithTags("Kontakty")
.Produces(StatusCodes.Status200OK)
.Produces(StatusCodes.Status400BadRequest);


app.MapPost("/api/contacts/emails",
    (AddEmailDto dto, IContactUseCases useCases) =>
    useCases
        .AddEmail(dto)
        .Match(
            _ => Results.Ok(),
            error => Results.Problem(detail: error, statusCode: 400)
        )
    )
.WithSummary("Dodaje e-mail do kontaktu")
.WithDescription("Dodaje nowy adres e-mail dla wskazanego kontaktu i kategorii.")
.WithTags("Kontakty")
.Produces(StatusCodes.Status200OK)
.Produces(StatusCodes.Status400BadRequest);


app.MapDelete("/api/contacts/emails",
    ([FromBody] RemoveEmailDto dto, IContactUseCases useCases) =>
    useCases
        .RemoveEmail(dto)
        .Match(
            _ => Results.Ok(),
            error => Results.Problem(detail: error, statusCode: 400)
        )
    )
.WithSummary("Usuwa e-mail z kontaktu")
.WithTags("Kontakty")
.Produces(StatusCodes.Status200OK)
.Produces(StatusCodes.Status400BadRequest);


app.MapPost("/api/contacts/phones",
    (AddPhoneDto dto, IContactUseCases useCases) =>
    useCases
        .AddPhone(dto)
        .Match(
            _ => Results.Ok(),
            error => Results.Problem(detail: error, statusCode: 400)
        )
    )
.WithSummary("Dodaje numer telefonu do kontaktu")
.WithTags("Kontakty")
.Produces(StatusCodes.Status200OK)
.Produces(StatusCodes.Status400BadRequest);


app.MapPut("/api/contacts",
    (UpdateContactDto dto, IContactUseCases useCases) =>
    useCases
        .UpdateContact(dto)
        .Match(
            _ => Results.Ok(),
            error => Results.Problem(detail: error, statusCode: 400)
        )
    )
.WithSummary("Aktualizuje podstawowe dane kontaktu")
.WithTags("Kontakty")
.Produces(StatusCodes.Status200OK)
.Produces(StatusCodes.Status400BadRequest);


app.MapDelete("/api/contacts/{id:guid}",
    (Guid id, IContactUseCases useCases) =>
    useCases
        .RemoveContact(id)
        .Match(
            _ => Results.Ok(),
            error => Results.Problem(detail: error, statusCode: 400)
        )
    )
.WithSummary("Usuwa kontakt o podanym ID")
.WithTags("Kontakty")
.Produces(StatusCodes.Status200OK)
.Produces(StatusCodes.Status400BadRequest);



app.MapPost("/api/contacts/important-dates",
    (AddImportantDateDto dto, IContactUseCases useCases) =>
    useCases
        .AddImportantDate(dto)
        .Match(
            _ => Results.Ok(),
            error => Results.Problem(detail: error, statusCode: 400)
        )
    )
.WithSummary("Dodaje ważną datę do kontaktu")
.WithTags("Kontakty")
.Produces(StatusCodes.Status200OK)
.Produces(StatusCodes.Status400BadRequest);


app.MapDelete("/api/contacts/important-dates",
    ([FromBody] RemoveImportantDateDto dto, IContactUseCases useCases) =>
    useCases
        .RemoveImportantDate(dto)
        .Match(
            _ => Results.Ok(),
            error => Results.Problem(detail: error, statusCode: 400)
        )
    )
.WithSummary("Usuwa ważną datę z kontaktu")
.WithTags("Kontakty")
.Produces(StatusCodes.Status200OK)
.Produces(StatusCodes.Status400BadRequest);

app.MapPost("/api/users", (AddUserDto dto, IUserUseCases userUseCases) =>
    userUseCases
        .AddUser(dto)
        .Match(
            user => Results.Created($"/api/users/{user.Id}", user),
            error => Results.Problem(detail: error, statusCode: 400)
        )
);

app.MapPost("/api/login", (LoginUserDto dto, IUserUseCases userUseCases) =>
    userUseCases.Login(dto).Match(
        loginData => Results.Ok(loginData.AccessToken),
        _ => Results.Unauthorized()
    )
);

builder.Services.AddAuthorizationBuilder()
.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));

app.MapDelete("/api/users/{id:guid}", (Guid id, IUserUseCases useCases) =>
    Results.NoContent()
)
.RequireAuthorization("AdminOnly");

// Uruchomienie aplikacji
app.Run();
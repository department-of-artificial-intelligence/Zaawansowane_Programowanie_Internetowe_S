using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Application.DomainServices;
using Application.OutputPorts;
using Application.Queries;
using Application.Queries.Dtos;
using Application.Repositories;
using Application.Services;
using Application.UseCases;
using Application.UseCases.Dtos;
using Data;
using WebAPI.Helpers;
using WebAPI.Inputs;

// Przygotowanie konfiguratora naszej aplikacji
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("default"))
);

builder.Services.AddScoped<ICategoryQueries, CategoryQueries>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<ICategoryUseCases, CategoryServices>();
builder.Services.AddScoped<CategoryUniquenessChecker>();
builder.Services.AddScoped<IContactRepository, ContactRepository>();
builder.Services.AddScoped<IContactUseCases, ContactServices>();
builder.Services.AddScoped<IUserQueries, UserQueries>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUserUseCases, UserServices>();
builder.Services.AddScoped<UserUniquenessChecker>();
builder.Services.AddScoped<ITokenGenerator, TokenGenerator>();
builder.Services.AddScoped<IPasswordHasher, PasswordHasher>();

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
builder.Services.AddAuthorizationBuilder()
    .AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
builder.Services.AddSingleton<IAuthorizationHandler, CategoryOwnerHandler>();

builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<WebAPI.GlobalExceptionHandler>();

// Dodanie usług koniecznych dla OpenApi
builder.Services.AddOpenApi(options =>
{
    options.AddDocumentTransformer((document, context, ct) =>
    {
        document.Components ??= new OpenApiComponents();
        document.Components.SecuritySchemes ??=
            new Dictionary<string, OpenApiSecurityScheme>();
        document.Components.SecuritySchemes["Bearer"] =
            new OpenApiSecurityScheme
        {
            Type = SecuritySchemeType.Http,
            Scheme = "bearer",
            BearerFormat = "JWT",
            Description = "Wklej token JWT otrzymany z /api/login"
        };
        document.SecurityRequirements =
        [
            new OpenApiSecurityRequirement
            {
                [new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                    }
                }] = []
            }
        ];
        return Task.CompletedTask;
    });
});

// Zbudowanie aplikacji na podstawie konfiguracji
var app = builder.Build();

app.UseExceptionHandler();

// Mapowanie endpoint OpenApi (tylko w przypadku trybu deweloperskiego).
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/openapi/v1.json", "API v1");
    });
}

// Uruchomienie przekierowania żądań z HTTP do HTTPS
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

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
        loginData => Results.Ok(loginData),
        _ => Results.Unauthorized()
    )
);

app.MapPost("/api/refresh", (RefreshTokenDto dto, IUserUseCases userUseCases) =>
    userUseCases.Refresh(dto).Match(
        loginData => Results.Ok(loginData),
        _ => Results.Unauthorized()
    )
);

app.MapGet("/api/users", (IUserQueries queries) =>
    Results.Ok(queries.GetUsers())
)
.RequireAuthorization("AdminOnly");

app.MapDelete("/api/users/{id:guid}", (Guid id, IUserUseCases userUseCases) =>
    userUseCases
        .RemoveUser(id)
        .Match(
            _ => Results.NoContent(),
            error => Results.Problem(detail: error, statusCode: 400)
        )
)
.RequireAuthorization("AdminOnly");

app.MapPut("/api/users/profile",
    (UpdateUserProfileRequest input,
     IUserUseCases userUseCases,
     ClaimsPrincipal user) =>
        userUseCases
            .UpdateProfile(new UpdateUserProfileDto(
                Guid.Parse(user.FindFirstValue(JwtRegisteredClaimNames.Sub)!),
                input.FirstName,
                input.LastName,
                input.BirthDate,
                input.Address)
            )
            .Match(
                _ => Results.NoContent(),
                error => Results.Problem(detail: error, statusCode: 400)
            )
)
.RequireAuthorization();

app.MapPut("/api/users/role", (ChangeUserRoleDto dto, IUserUseCases userUseCases) =>
    userUseCases
        .ChangeRole(dto)
        .Match(
            _ => Results.NoContent(),
            error => Results.Problem(detail: error, statusCode: 400)
        )
)
.RequireAuthorization("AdminOnly");

var categories = app.MapGroup("/api/categories")
    .WithTags("Kategorie")
    .RequireAuthorization();

categories.MapPost("/",
    (AddCategoryRequest input,
     ICategoryUseCases useCases,
     ClaimsPrincipal user) =>
        useCases
            .AddCategory(new AddCategoryDto(
                input.Name,
                Guid.Parse(user.FindFirstValue(JwtRegisteredClaimNames.Sub)!))
            )
            .Match(
                category => Results.Created(
                    $"/api/categories/{category.Id}",
                    category),
                error => Results.Problem(detail: error, statusCode: 400)
            )
)
.WithSummary("Dodaje kategorię")
.WithDescription("Tworzy nową kategorię i zwraca utworzony obiekt.")
.Produces<AddCategoryResult>(StatusCodes.Status201Created)
.Produces(StatusCodes.Status400BadRequest);

categories.MapGet("/",
    (ICategoryQueries queries, ClaimsPrincipal user) =>
        Results.Ok(queries.GetCategories(
            Guid.Parse(user.FindFirstValue(JwtRegisteredClaimNames.Sub)!)))
)
.WithSummary("Pobiera wszystkie kategorie")
.WithDescription("Zwraca listę wszystkich kategorii zalogowanego użytkownika.")
.Produces<IEnumerable<CategoryDto>>(StatusCodes.Status200OK);

categories.MapGet("/{id:guid}",
    (Guid id, ICategoryQueries queries, ClaimsPrincipal user) =>
        queries
            .GetCategory(
                id,
                Guid.Parse(user.FindFirstValue(JwtRegisteredClaimNames.Sub)!))
            .Match<IResult>(
                category => Results.Ok(category),
                () => Results.NotFound()
            )
)
.WithSummary("Pobiera kategorię")
.WithDescription("Zwraca kategorię zalogowanego użytkownika o podanym identyfikatorze.")
.Produces<CategoryDto>(StatusCodes.Status200OK)
.Produces(StatusCodes.Status404NotFound);

categories.MapPut("/",
    async (UpdateCategoryDto dto,
           ICategoryUseCases useCases,
           ICategoryQueries queries,
           IAuthorizationService authorization,
           ClaimsPrincipal user) =>
    {
        var allowed = await queries
            .GetCategoryOwnerId(dto.Id)
            .Match(
                async ownerId => (await authorization.AuthorizeAsync(
                    user, ownerId, new CategoryOwnerRequirement())).Succeeded,
                () => Task.FromResult(true)
            );

        if (!allowed)
        {
            return Results.Forbid();
        }

        return useCases
            .UpdateCategory(dto)
            .Match(
                _ => Results.NoContent(),
                error => Results.Problem(detail: error, statusCode: 400)
            );
    }
)
.WithSummary("Zmienia nazwę kategorii")
.WithDescription("Zmienia nazwę istniejącej kategorii na podaną. Dostępne dla właściciela kategorii i administratora.")
.Produces(StatusCodes.Status204NoContent)
.Produces(StatusCodes.Status400BadRequest);

categories.MapDelete("/{id:guid}",
    async (Guid id,
           ICategoryUseCases useCases,
           ICategoryQueries queries,
           IAuthorizationService authorization,
           ClaimsPrincipal user) =>
    {
        var allowed = await queries
            .GetCategoryOwnerId(id)
            .Match(
                async ownerId => (await authorization.AuthorizeAsync(
                    user, ownerId, new CategoryOwnerRequirement())).Succeeded,
                () => Task.FromResult(true)
            );

        if (!allowed)
        {
            return Results.Forbid();
        }

        return useCases
            .RemoveCategory(id)
            .Match(
                _ => Results.NoContent(),
                error => Results.Problem(detail: error, statusCode: 400)
            );
    }
)
.WithSummary("Usuwa kategorię")
.WithDescription("Usuwa kategorię, o ile nie jest przypisana do żadnego adresu e-mail. Dostępne dla właściciela kategorii i administratora.")
.Produces(StatusCodes.Status204NoContent)
.Produces(StatusCodes.Status400BadRequest);

app.MapPost("/api/contacts",
    (AddContactRequest input,
     IContactUseCases useCases,
     ClaimsPrincipal user) =>
        useCases
            .AddContact(new AddContactDto(
                input.FirstName,
                input.LastName,
                input.Sex,
                input.Age,
                Guid.Parse(user.FindFirstValue(JwtRegisteredClaimNames.Sub)!))
            )
            .Match(
                contact => Results.Created(
                    $"/api/contacts/{contact.Id}",
                    contact),
                error => Results.Problem(detail: error, statusCode: 400)
            )
)
.RequireAuthorization()
.WithSummary("Dodaje kontakt")
.WithDescription("Tworzy nowy kontakt i zwraca utworzony obiekt.")
.WithTags("Kontakty")
.Produces<AddContactResult>(StatusCodes.Status201Created)
.Produces(StatusCodes.Status400BadRequest);

app.MapPut("/api/contacts",
    (UpdateContactDto dto, IContactUseCases useCases) =>
        useCases
            .UpdateContact(dto)
            .Match(
                _ => Results.NoContent(),
                error => Results.Problem(detail: error, statusCode: 400)
            )
)
.RequireAuthorization()
.WithSummary("Zmienia dane kontaktu")
.WithDescription("Zmienia imię, nazwisko, płeć i wiek istniejącego kontaktu.")
.WithTags("Kontakty")
.Produces(StatusCodes.Status204NoContent)
.Produces(StatusCodes.Status400BadRequest);

app.MapDelete("/api/contacts/{id:guid}",
    (Guid id, IContactUseCases useCases) =>
        useCases
            .RemoveContact(id)
            .Match(
                _ => Results.NoContent(),
                error => Results.Problem(detail: error, statusCode: 400)
            )
)
.RequireAuthorization()
.WithSummary("Usuwa kontakt")
.WithDescription("Usuwa kontakt razem z jego adresami e-mail, numerami telefonu i ważnymi datami.")
.WithTags("Kontakty")
.Produces(StatusCodes.Status204NoContent)
.Produces(StatusCodes.Status400BadRequest);

app.MapPost("/api/contacts/emails",
    (AddEmailDto dto, IContactUseCases useCases) =>
        useCases
            .AddEmail(dto)
            .Match(
                _ => Results.NoContent(),
                error => Results.Problem(detail: error, statusCode: 400)
            )
)
.RequireAuthorization()
.WithSummary("Dodaje adres e-mail do kontaktu")
.WithDescription("Dołącza do kontaktu adres e-mail przypisany do wskazanej kategorii.")
.WithTags("Kontakty")
.Produces(StatusCodes.Status204NoContent)
.Produces(StatusCodes.Status400BadRequest);

app.MapDelete("/api/contacts/{contactId:guid}/emails/{address}",
    (Guid contactId, string address, IContactUseCases useCases) =>
        useCases
            .RemoveEmail(new RemoveEmailDto(contactId, address))
            .Match(
                _ => Results.NoContent(),
                error => Results.Problem(detail: error, statusCode: 400)
            )
)
.RequireAuthorization()
.WithSummary("Usuwa adres e-mail z kontaktu")
.WithDescription("Usuwa z kontaktu podany adres e-mail.")
.WithTags("Kontakty")
.Produces(StatusCodes.Status204NoContent)
.Produces(StatusCodes.Status400BadRequest);

app.MapPost("/api/contacts/phones",
    (AddPhoneNumberDto dto, IContactUseCases useCases) =>
        useCases
            .AddPhoneNumber(dto)
            .Match(
                _ => Results.NoContent(),
                error => Results.Problem(detail: error, statusCode: 400)
            )
)
.RequireAuthorization()
.WithSummary("Dodaje numer telefonu do kontaktu")
.WithDescription("Dołącza do kontaktu numer telefonu.")
.WithTags("Kontakty")
.Produces(StatusCodes.Status204NoContent)
.Produces(StatusCodes.Status400BadRequest);

app.MapDelete("/api/contacts/{contactId:guid}/phones/{number}",
    (Guid contactId, string number, IContactUseCases useCases) =>
        useCases
            .RemovePhoneNumber(new RemovePhoneNumberDto(contactId, number))
            .Match(
                _ => Results.NoContent(),
                error => Results.Problem(detail: error, statusCode: 400)
            )
)
.RequireAuthorization()
.WithSummary("Usuwa numer telefonu z kontaktu")
.WithDescription("Usuwa z kontaktu podany numer telefonu.")
.WithTags("Kontakty")
.Produces(StatusCodes.Status204NoContent)
.Produces(StatusCodes.Status400BadRequest);

app.MapPost("/api/contacts/important-dates",
    (AddImportantDateDto dto, IContactUseCases useCases) =>
        useCases
            .AddImportantDate(dto)
            .Match(
                _ => Results.NoContent(),
                error => Results.Problem(detail: error, statusCode: 400)
            )
)
.RequireAuthorization()
.WithSummary("Dodaje ważną datę do kontaktu")
.WithDescription("Dołącza do kontaktu ważną datę z opisem, np. urodziny.")
.WithTags("Kontakty")
.Produces(StatusCodes.Status204NoContent)
.Produces(StatusCodes.Status400BadRequest);

app.MapDelete("/api/contacts/{contactId:guid}/important-dates",
    (Guid contactId, string description, DateOnly date, IContactUseCases useCases) =>
        useCases
            .RemoveImportantDate(
                new RemoveImportantDateDto(contactId, description, date))
            .Match(
                _ => Results.NoContent(),
                error => Results.Problem(detail: error, statusCode: 400)
            )
)
.RequireAuthorization()
.WithSummary("Usuwa ważną datę z kontaktu")
.WithDescription("Usuwa z kontaktu ważną datę o podanym opisie i dniu.")
.WithTags("Kontakty")
.Produces(StatusCodes.Status204NoContent)
.Produces(StatusCodes.Status400BadRequest);

// Uruchomienie aplikacji
app.Run();

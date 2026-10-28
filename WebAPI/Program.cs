using System.Text;
using System.Text.Json.Serialization;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Application.DomainServices;
using Application.OutputPorts;
using Application.Queries;
using Application.Queries.Dtos;
using Application.Repositories;
using Application.Services;
using Application.UseCases;
using Application.UseCases.Dtos;
using Data;
using WebAPI;
using WebAPI.Helpers;


// Przygotowanie konfiguratora naszej aplikacji
var builder = WebApplication.CreateBuilder(args);

// Dodanie kontekstu danych
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("default"))
);

// Rejestracja zależności w kontenerze Dependency Injection
// Zapytania
builder.Services.AddScoped<ICategoryQueries, CategoryQueries>();
builder.Services.AddScoped<IContactQueries, ContactQueries>();
// Porty wyjściowe i repozytoria
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<IContactRepository, ContactRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<ITokenGenerator, TokenGenerator>();
// Usługi domenowe
builder.Services.AddScoped<CategoryUniquenessChecker>();
builder.Services.AddScoped<CategoryUsageChecker>();
// Przypadki użycia
builder.Services.AddScoped<ICategoryUseCases, CategoryServices>();
builder.Services.AddScoped<IContactUseCases, ContactServices>();
builder.Services.AddScoped<IUserUseCases, UserServices>();

// Enumy (Sex, PhoneType) przesyłane w JSON jako tekst ("Male", "Mobile") zamiast liczb
builder.Services.ConfigureHttpJsonOptions(options =>
    options.SerializerOptions.Converters.Add(new JsonStringEnumConverter()));

// Globalna obsługa wyjątków (Problem Details, RFC 7807)
builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

// Autentykacja JWT
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(x =>
    {
        x.TokenValidationParameters = new TokenValidationParameters
        {
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

// Dodanie usług koniecznych dla OpenApi
builder.Services.AddOpenApi();

// Zbudowanie aplikacji na podstawie konfiguracji
var app = builder.Build();

app.UseExceptionHandler();

// Mapowanie endpoint OpenApi i Swagger UI (tylko w przypadku trybu deweloperskiego).
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


// Autentykacja i autoryzacja (kolejność ma znaczenie)
app.UseAuthentication();
app.UseAuthorization();



// =====================================================================
// Kategorie
// =====================================================================
var categories = app.MapGroup("/api/categories")
    .WithTags("Kategorie")
    .ProducesProblem(StatusCodes.Status500InternalServerError);

categories.MapGet("",
    (ICategoryQueries queries) => Results.Ok(queries.GetCategories())
)
.RequireAuthorization()
.WithSummary("Pobiera wszystkie kategorie")
.WithDescription("Zwraca listę wszystkich kategorii zapisanych w bazie danych.")
.Produces<IEnumerable<CategoryDto>>(StatusCodes.Status200OK);

categories.MapGet("/{id:guid}",
    (Guid id, ICategoryQueries queries) =>
        queries
            .GetCategory(id)
            .Match(
                category => Results.Ok(category),
                () => Results.Problem(
                    detail: $"Nie mogę odnaleźć kategorii o id = {id}",
                    statusCode: StatusCodes.Status404NotFound)
            )
)
.WithSummary("Pobiera kategorię")
.WithDescription("Zwraca kategorię o podanym identyfikatorze.")
.Produces<CategoryDto>(StatusCodes.Status200OK)
.ProducesProblem(StatusCodes.Status404NotFound);

categories.MapPost("",
    (AddCategoryDto dto, ICategoryUseCases useCases) =>
        useCases
            .AddCategory(dto)
            .Match(
                category => Results.Created(
                    $"/api/categories/{category.Id}",
                    category),
                error => Results.Problem(detail: error, statusCode: 400)
            )
)
.WithSummary("Dodaje kategorię")
.WithDescription("Tworzy nową kategorię i zwraca utworzony obiekt. " +
                 "Nazwa kategorii nie może być pusta i musi być unikalna.")
.Produces<AddCategoryResult>(StatusCodes.Status201Created)
.ProducesProblem(StatusCodes.Status400BadRequest);

categories.MapPut("/{id:guid}",
    (Guid id, RenameCategoryRequest request, ICategoryUseCases useCases) =>
        useCases
            .RenameCategory(new RenameCategoryDto(id, request.Name))
            .ToNoContent()
)
.WithSummary("Zmienia nazwę kategorii")
.WithDescription("Zmienia nazwę kategorii o podanym identyfikatorze. " +
                 "Nowa nazwa nie może być pusta ani zajęta przez inną kategorię.")
.Produces(StatusCodes.Status204NoContent)
.ProducesProblem(StatusCodes.Status400BadRequest);

categories.MapDelete("/{id:guid}",
    (Guid id, ICategoryUseCases useCases) =>
        useCases
            .RemoveCategory(id)
            .ToNoContent()
)
.WithSummary("Usuwa kategorię")
.WithDescription("Usuwa kategorię o podanym identyfikatorze. " +
                 "Nie można usunąć kategorii przypisanej do jakiegokolwiek adresu e-mail.")
.Produces(StatusCodes.Status204NoContent)
.ProducesProblem(StatusCodes.Status400BadRequest);

// =====================================================================
// Kontakty
// =====================================================================
var contacts = app.MapGroup("/api/contacts")
    .WithTags("Kontakty")
    .ProducesProblem(StatusCodes.Status500InternalServerError);

contacts.MapGet("",
    (IContactQueries queries) => Results.Ok(queries.GetContacts())
)
.WithSummary("Pobiera wszystkie kontakty")
.WithDescription("Zwraca listę wszystkich kontaktów wraz z adresami e-mail, telefonami i ważnymi datami.")
.Produces<IEnumerable<ContactDto>>(StatusCodes.Status200OK);

contacts.MapGet("/{id:guid}",
    (Guid id, IContactQueries queries) =>
        queries
            .GetContact(id)
            .Match(
                contact => Results.Ok(contact),
                () => Results.Problem(
                    detail: $"Brak kontaktu (id: {id})",
                    statusCode: StatusCodes.Status404NotFound)
            )
)
.WithSummary("Pobiera kontakt")
.WithDescription("Zwraca kontakt o podanym identyfikatorze wraz z adresami e-mail, telefonami i ważnymi datami.")
.Produces<ContactDto>(StatusCodes.Status200OK)
.ProducesProblem(StatusCodes.Status404NotFound);

contacts.MapPost("",
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
.WithDescription("Tworzy nowy kontakt i zwraca utworzony obiekt. " +
                 "Imię (maks. 50 znaków) i nazwisko (maks. 100 znaków) są wymagane, wiek musi mieścić się w zakresie 18–120.")
.Produces<AddContactResult>(StatusCodes.Status201Created)
.ProducesProblem(StatusCodes.Status400BadRequest);

contacts.MapPut("/{id:guid}",
    (Guid id, UpdateContactRequest request, IContactUseCases useCases) =>
        useCases
            .UpdateContact(new UpdateContactDto(
                id, request.FirstName, request.LastName, request.Sex, request.Age))
            .ToNoContent()
)
.WithSummary("Aktualizuje kontakt")
.WithDescription("Zmienia imię, nazwisko, płeć i wiek kontaktu o podanym identyfikatorze.")
.Produces(StatusCodes.Status204NoContent)
.ProducesProblem(StatusCodes.Status400BadRequest);

contacts.MapDelete("/{id:guid}",
    (Guid id, IContactUseCases useCases) =>
        useCases
            .RemoveContact(id)
            .ToNoContent()
)
.WithSummary("Usuwa kontakt")
.WithDescription("Usuwa kontakt o podanym identyfikatorze wraz z jego adresami e-mail, telefonami i ważnymi datami.")
.Produces(StatusCodes.Status204NoContent)
.ProducesProblem(StatusCodes.Status400BadRequest);

// ---------- E-maile ----------

contacts.MapPost("/{id:guid}/emails",
    (Guid id, AddEmailRequest request, IContactUseCases useCases) =>
        useCases
            .AddEmail(new AddEmailDto(id, request.Address, request.CategoryId))
            .ToNoContent()
)
.WithSummary("Dodaje adres e-mail do kontaktu")
.WithDescription("Dodaje adres e-mail przypisany do istniejącej kategorii. " +
                 "Adres musi mieć poprawny format i nie może się powtarzać w kontakcie.")
.Produces(StatusCodes.Status204NoContent)
.ProducesProblem(StatusCodes.Status400BadRequest);

contacts.MapDelete("/{id:guid}/emails/{address}",
    (Guid id, string address, IContactUseCases useCases) =>
        useCases
            .RemoveEmail(new RemoveEmailDto(id, address))
            .ToNoContent()
)
.WithSummary("Usuwa adres e-mail z kontaktu")
.WithDescription("Usuwa z kontaktu podany adres e-mail (wielkość liter nie ma znaczenia).")
.Produces(StatusCodes.Status204NoContent)
.ProducesProblem(StatusCodes.Status400BadRequest);

// ---------- Telefony ----------

contacts.MapPost("/{id:guid}/phones",
    (Guid id, AddPhoneRequest request, IContactUseCases useCases) =>
        useCases
            .AddPhone(new AddPhoneDto(id, request.Number, request.Type))
            .ToNoContent()
)
.WithSummary("Dodaje numer telefonu do kontaktu")
.WithDescription("Dodaje numer telefonu (9–15 cyfr, opcjonalnie z '+'; spacje, myślniki i nawiasy są pomijane). " +
                 "Typ: Mobile, Home, Work lub Other.")
.Produces(StatusCodes.Status204NoContent)
.ProducesProblem(StatusCodes.Status400BadRequest);

contacts.MapDelete("/{id:guid}/phones/{number}",
    (Guid id, string number, IContactUseCases useCases) =>
        useCases
            .RemovePhone(new RemovePhoneDto(id, number))
            .ToNoContent()
)
.WithSummary("Usuwa numer telefonu z kontaktu")
.WithDescription("Usuwa z kontaktu podany numer telefonu.")
.Produces(StatusCodes.Status204NoContent)
.ProducesProblem(StatusCodes.Status400BadRequest);

// ---------- Ważne daty ----------

contacts.MapPost("/{id:guid}/important-dates",
    (Guid id, AddImportantDateRequest request, IContactUseCases useCases) =>
        useCases
            .AddImportantDate(new AddImportantDateDto(id, request.Date, request.Label))
            .ToNoContent()
)
.WithSummary("Dodaje ważną datę do kontaktu")
.WithDescription("Dodaje ważną datę (np. urodziny). Nazwa daty musi być unikalna w kontakcie, " +
                 "a data nie może być wcześniejsza niż 01.01.1900.")
.Produces(StatusCodes.Status204NoContent)
.ProducesProblem(StatusCodes.Status400BadRequest);

contacts.MapDelete("/{id:guid}/important-dates/{label}",
    (Guid id, string label, IContactUseCases useCases) =>
        useCases
            .RemoveImportantDate(new RemoveImportantDateDto(id, label))
            .ToNoContent()
)
.WithSummary("Usuwa ważną datę z kontaktu")
.WithDescription("Usuwa z kontaktu ważną datę o podanej nazwie (wielkość liter nie ma znaczenia).")
.Produces(StatusCodes.Status204NoContent)
.ProducesProblem(StatusCodes.Status400BadRequest);


// =====================================================================
// Użytkownicy
// =====================================================================
var users = app.MapGroup("/api/users")
    .WithTags("Użytkownicy")
    .ProducesProblem(StatusCodes.Status500InternalServerError);

users.MapPost("",
    (AddUserDto dto, IUserUseCases userUseCases) =>
        userUseCases
            .AddUser(dto)
            .Match(
                user => Results.Created($"/api/users/{user.Id}", user),
                error => Results.Problem(detail: error, statusCode: 400)
            )
)
.WithSummary("Rejestruje użytkownika")
.WithDescription("Tworzy nowego użytkownika z podaną nazwą i hasłem.")
.Produces<AddUserResult>(StatusCodes.Status201Created)
.ProducesProblem(StatusCodes.Status400BadRequest);

app.MapPost("/api/login",
    (LoginUserDto dto, IUserUseCases userUseCases) =>
        userUseCases
            .Login(dto)
            .Match(
                loginData => Results.Ok(loginData),
                _ => Results.Unauthorized()
            )
)
.WithTags("Użytkownicy")
.WithSummary("Logowanie")
.WithDescription("Zwraca token JWT (accessToken) dla poprawnych danych logowania.")
.Produces<LoginUserResult>(StatusCodes.Status200OK)
.Produces(StatusCodes.Status401Unauthorized);
// Uruchomienie aplikacji
app.Run();
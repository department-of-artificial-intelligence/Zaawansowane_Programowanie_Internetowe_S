using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using Application.DomainServices;
using Application.OutputPorts;
using Application.Queries;
using Application.Queries.Dtos;
using Application.Repositories;
using Application.Services;
using Application.UseCases;
using Application.UseCases.Dtos;
using Application.Utilities;
using Data;
using WebAPI.Authorization;
using WebAPI.Helpers;
using WebAPI.Inputs;

// Przygotowanie konfiguratora naszej aplikacji
var builder = WebApplication.CreateBuilder(args);

// Dodanie usług koniecznych dla OpenApi wraz z opisem schematu uwierzytelniania,
// dzięki któremu Swagger UI wyświetla przycisk "Authorize".
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

// Uwierzytelnianie za pomocą tokenów JWT
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
        // Zadanie 10 - domyślna tolerancja (5 minut) wydłużałaby krótki czas życia tokenu.
        ClockSkew = TimeSpan.Zero,
        NameClaimType = "name",
        RoleClaimType = "role"
    };
    x.MapInboundClaims = false;
});

// Nazwane polityki autoryzacji
builder.Services.AddAuthorizationBuilder()
    .AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"))
    .AddPolicy("CategoryOwner", policy =>
        policy.AddRequirements(new CategoryOwnerRequirement()));
builder.Services.AddSingleton<IAuthorizationHandler, CategoryOwnerHandler>();

// Dodanie kontekstu danych
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("default"))
);

builder.Services.AddScoped<ICategoryQueries, CategoryQueries>();
builder.Services.AddScoped<IContactQueries, ContactQueries>();
builder.Services.AddScoped<IUserQueries, UserQueries>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<IContactRepository, ContactRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<ICategoryUseCases, CategoryServices>();
builder.Services.AddScoped<IContactUseCases, ContactServices>();
builder.Services.AddScoped<IUserUseCases, UserServices>();
builder.Services.AddScoped<CategoryUniquenessChecker>();
builder.Services.AddScoped<UserNameUniquenessChecker>();
builder.Services.AddScoped<ITokenGenerator, TokenGenerator>();
builder.Services.AddSingleton<IPasswordHasher, PasswordHasher>();
builder.Services.AddSingleton<IClock, SystemClock>();

// Zbudowanie aplikacji na podstawie konfiguracji
var app = builder.Build();

// Zadanie 6 - utworzenie bazy danych oraz pierwszego administratora
// (jeżeli jego dane zostały podane w konfiguracji).
using (var scope = app.Services.CreateScope())
{
    scope.ServiceProvider.GetRequiredService<AppDbContext>().Database.Migrate();

    var adminName = app.Configuration["ApplicationSettings:ADMIN_USERNAME"];
    var adminPassword = app.Configuration["ApplicationSettings:ADMIN_PASSWORD"];
    if (!string.IsNullOrEmpty(adminName) && !string.IsNullOrEmpty(adminPassword))
    {
        scope.ServiceProvider
            .GetRequiredService<IUserUseCases>()
            .EnsureAdminExists(new AddUserDto(adminName, adminPassword))
            .Match(
                _ => Unit.Instance,
                error => throw new InvalidOperationException(
                    $"Nie udało się utworzyć administratora: {error}"));
    }
}

app.UseExceptionHandler();

// Mapowanie endpoint OpenApi oraz Swagger UI (tylko w przypadku trybu deweloperskiego).
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

// ---------------------------------------------------------------------------
// Uwierzytelnianie
// ---------------------------------------------------------------------------
var auth = app
    .MapGroup("/api")
    .WithTags("Uwierzytelnianie")
    .ProducesProblem(StatusCodes.Status500InternalServerError);

auth.MapPost("/login",
        (LoginUserDto dto, IUserUseCases userUseCases) =>
            userUseCases.Login(dto).Match(
                loginData => Results.Ok(loginData),
                _ => Results.Unauthorized()
            ))
    .WithSummary("Logowanie")
    .WithDescription("Zwraca token dostępowy (ważny kilkanaście minut) oraz refresh token.")
    .Produces<LoginUserResult>(StatusCodes.Status200OK)
    .Produces(StatusCodes.Status401Unauthorized);

auth.MapPost("/refresh",
        (RefreshTokenRequest request, IUserUseCases userUseCases) =>
            userUseCases.Refresh(request.RefreshToken).Match(
                loginData => Results.Ok(loginData),
                _ => Results.Unauthorized()
            ))
    .WithSummary("Odświeża token dostępowy")
    .WithDescription("Wymienia refresh token na nową parę tokenów. Poprzedni refresh token przestaje być ważny.")
    .Produces<LoginUserResult>(StatusCodes.Status200OK)
    .Produces(StatusCodes.Status401Unauthorized);

auth.MapPost("/logout",
        (ClaimsPrincipal user, IUserUseCases userUseCases) =>
            NoContent(userUseCases.Logout(user.GetUserId())))
    .RequireAuthorization()
    .WithSummary("Wylogowanie")
    .WithDescription("Unieważnia refresh token zalogowanego użytkownika.")
    .Produces(StatusCodes.Status204NoContent)
    .Produces(StatusCodes.Status401Unauthorized);

// ---------------------------------------------------------------------------
// Użytkownicy (zadania 5 i 6)
// ---------------------------------------------------------------------------
var users = app
    .MapGroup("/api/users")
    .WithTags("Użytkownicy")
    .RequireAuthorization()
    .ProducesProblem(StatusCodes.Status500InternalServerError);

users.MapPost("/",
        (AddUserDto dto, IUserUseCases userUseCases) =>
            userUseCases
                .AddUser(dto)
                .Match(
                    user => Results.Created($"/api/users/{user.Id}", user),
                    error => Results.Problem(detail: error, statusCode: 400)
                ))
    .AllowAnonymous()
    .WithSummary("Rejestruje użytkownika")
    .WithDescription("Tworzy nowe konto z rolą User. Login musi być unikalny, " +
                     "hasło: min. 8 znaków, wielka i mała litera oraz cyfra.")
    .Produces<AddUserResult>(StatusCodes.Status201Created)
    .ProducesProblem(StatusCodes.Status400BadRequest);

users.MapGet("/me",
        (ClaimsPrincipal user, IUserQueries queries) =>
            queries
                .GetUser(user.GetUserId())
                .Match(
                    found => Results.Ok(found),
                    () => Results.NotFound()))
    .WithSummary("Pobiera profil zalogowanego użytkownika")
    .Produces<UserDto>(StatusCodes.Status200OK)
    .Produces(StatusCodes.Status404NotFound);

users.MapPut("/me/profile",
        (UpdateProfileRequest request, ClaimsPrincipal user, IUserUseCases userUseCases) =>
            NoContent(userUseCases.UpdateProfile(new UpdateProfileDto(
                user.GetUserId(), request.FirstName, request.LastName,
                request.DateOfBirth, request.Address))))
    .WithSummary("Modyfikuje profil zalogowanego użytkownika")
    .WithDescription("Ustawia imię, nazwisko, datę urodzenia i adres. Puste pole usuwa wartość.")
    .Produces(StatusCodes.Status204NoContent)
    .ProducesProblem(StatusCodes.Status400BadRequest);

users.MapPut("/me/password",
        (ChangePasswordRequest request, ClaimsPrincipal user, IUserUseCases userUseCases) =>
            NoContent(userUseCases.ChangePassword(new ChangePasswordDto(
                user.GetUserId(), request.CurrentPassword, request.NewPassword))))
    .WithSummary("Zmienia hasło zalogowanego użytkownika")
    .Produces(StatusCodes.Status204NoContent)
    .ProducesProblem(StatusCodes.Status400BadRequest);

users.MapGet("/",
        (IUserQueries queries) => Results.Ok(queries.GetUsers()))
    .RequireAuthorization("AdminOnly")
    .WithSummary("Pobiera listę użytkowników (administrator)")
    .Produces<IEnumerable<UserDto>>(StatusCodes.Status200OK)
    .Produces(StatusCodes.Status403Forbidden);

users.MapGet("/{id:guid}",
        (Guid id, IUserQueries queries) =>
            queries
                .GetUser(id)
                .Match(
                    found => Results.Ok(found),
                    () => Results.NotFound()))
    .RequireAuthorization("AdminOnly")
    .WithSummary("Pobiera użytkownika (administrator)")
    .Produces<UserDto>(StatusCodes.Status200OK)
    .Produces(StatusCodes.Status403Forbidden)
    .Produces(StatusCodes.Status404NotFound);

users.MapPut("/{id:guid}/role",
        (Guid id, ChangeRoleRequest request, ClaimsPrincipal user, IUserUseCases userUseCases) =>
            NoContent(userUseCases.ChangeRole(
                new ChangeRoleDto(id, request.Role, user.GetUserId()))))
    .RequireAuthorization("AdminOnly")
    .WithSummary("Zmienia rolę użytkownika (administrator)")
    .WithDescription("Rola: 0 - User, 1 - Admin. Zmiana obowiązuje od następnego " +
                     "wygenerowania tokenu (logowanie lub odświeżenie).")
    .Produces(StatusCodes.Status204NoContent)
    .Produces(StatusCodes.Status403Forbidden)
    .ProducesProblem(StatusCodes.Status400BadRequest);

users.MapDelete("/{id:guid}",
        (Guid id, ClaimsPrincipal user, IUserUseCases userUseCases) =>
            NoContent(userUseCases.RemoveUser(id, user.GetUserId())))
    .RequireAuthorization("AdminOnly")
    .WithSummary("Usuwa użytkownika (administrator)")
    .WithDescription("Usuwa użytkownika wraz z jego kategoriami i kontaktami.")
    .Produces(StatusCodes.Status204NoContent)
    .Produces(StatusCodes.Status403Forbidden)
    .ProducesProblem(StatusCodes.Status400BadRequest);

// ---------------------------------------------------------------------------
// Kategorie
// ---------------------------------------------------------------------------
var categories = app
    .MapGroup("/api/categories")
    .WithTags("Kategorie")
    .RequireAuthorization()
    .ProducesProblem(StatusCodes.Status500InternalServerError);

categories.MapGet("/",
        (ICategoryQueries queries, ClaimsPrincipal user) =>
            Results.Ok(queries.GetCategories(user.GetUserId())))
    .WithSummary("Pobiera kategorie zalogowanego użytkownika")
    .Produces<IEnumerable<CategoryDto>>(StatusCodes.Status200OK);

// Zadanie 9 - cudza kategoria jest traktowana jak nieistniejąca.
categories.MapGet("/{categoryId:guid}",
        (Guid categoryId, ICategoryQueries queries, ClaimsPrincipal user) =>
            queries
                .GetCategory(categoryId, user.GetUserId())
                .Match(
                    category => Results.Ok(category),
                    () => Results.NotFound()))
    .WithSummary("Pobiera kategorię")
    .WithDescription("Zwraca kategorię zalogowanego użytkownika o podanym identyfikatorze.")
    .Produces<CategoryDto>(StatusCodes.Status200OK)
    .Produces(StatusCodes.Status404NotFound);

categories.MapPost("/",
        (AddCategoryRequest input, ICategoryUseCases useCases, ClaimsPrincipal user) =>
            useCases
                .AddCategory(new AddCategoryDto(input.Name, user.GetUserId()))
                .Match(
                    category => Results.Created(
                        $"/api/categories/{category.Id}",
                        category),
                    error => Results.Problem(detail: error, statusCode: 400)
                ))
    .WithSummary("Dodaje kategorię")
    .WithDescription("Tworzy nową kategorię zalogowanego użytkownika. " +
                     "Nazwa musi być niepusta i unikalna wśród jego kategorii.")
    .Produces<AddCategoryResult>(StatusCodes.Status201Created)
    .ProducesProblem(StatusCodes.Status400BadRequest);

// Zadanie 11 - modyfikować i usuwać kategorię może tylko jej właściciel lub administrator.
categories.MapPut("/{categoryId:guid}",
        (Guid categoryId, NameRequest request, ClaimsPrincipal user,
         ICategoryQueries queries, IAuthorizationService authorization,
         ICategoryUseCases useCases) =>
            AuthorizeCategory(categoryId, user, queries, authorization,
                () => NoContent(useCases.RenameCategory(categoryId, request.Name))))
    .WithSummary("Zmienia nazwę kategorii")
    .WithDescription("Zmienia nazwę kategorii. Nowa nazwa musi być niepusta i unikalna.")
    .Produces(StatusCodes.Status204NoContent)
    .Produces(StatusCodes.Status403Forbidden)
    .Produces(StatusCodes.Status404NotFound)
    .ProducesProblem(StatusCodes.Status400BadRequest);

categories.MapDelete("/{categoryId:guid}",
        (Guid categoryId, ClaimsPrincipal user,
         ICategoryQueries queries, IAuthorizationService authorization,
         ICategoryUseCases useCases) =>
            AuthorizeCategory(categoryId, user, queries, authorization,
                () => NoContent(useCases.RemoveCategory(categoryId))))
    .WithSummary("Usuwa kategorię")
    .WithDescription("Usuwa kategorię. Nie można usunąć kategorii przypisanej do adresu email.")
    .Produces(StatusCodes.Status204NoContent)
    .Produces(StatusCodes.Status403Forbidden)
    .Produces(StatusCodes.Status404NotFound)
    .ProducesProblem(StatusCodes.Status400BadRequest);

// ---------------------------------------------------------------------------
// Kontakty (zadanie 8 - każdy użytkownik zarządza wyłącznie swoimi kontaktami)
// ---------------------------------------------------------------------------
var contacts = app
    .MapGroup("/api/contacts")
    .WithTags("Kontakty")
    .RequireAuthorization()
    .ProducesProblem(StatusCodes.Status500InternalServerError);

contacts.MapGet("/",
        (IContactQueries queries, ClaimsPrincipal user) =>
            Results.Ok(queries.GetContacts(user.GetUserId())))
    .WithSummary("Pobiera kontakty zalogowanego użytkownika")
    .Produces<IEnumerable<ContactDto>>(StatusCodes.Status200OK);

contacts.MapGet("/{contactId:guid}",
        (Guid contactId, IContactQueries queries, ClaimsPrincipal user) =>
            queries
                .GetContact(contactId, user.GetUserId())
                .Match(
                    contact => Results.Ok(contact),
                    () => Results.NotFound()))
    .WithSummary("Pobiera kontakt")
    .Produces<ContactDto>(StatusCodes.Status200OK)
    .Produces(StatusCodes.Status404NotFound);

contacts.MapPost("/",
        (AddContactRequest request, IContactUseCases useCases, ClaimsPrincipal user) =>
            useCases
                .AddContact(new AddContactDto(
                    request.FirstName, request.LastName, request.Sex, request.Age,
                    user.GetUserId()))
                .Match(
                    contact => Results.Created(
                        $"/api/contacts/{contact.Id}",
                        contact),
                    error => Results.Problem(detail: error, statusCode: 400)
                ))
    .WithSummary("Dodaje kontakt")
    .WithDescription("Tworzy nowy kontakt i zwraca utworzony obiekt. " +
                     "Płeć: 0 - Male, 1 - Female. Wiek musi być z zakresu 18-120.")
    .Produces<AddContactResult>(StatusCodes.Status201Created)
    .ProducesProblem(StatusCodes.Status400BadRequest);

contacts.MapPut("/{contactId:guid}",
        (Guid contactId, UpdateContactRequest request, IContactUseCases useCases, ClaimsPrincipal user) =>
            NoContent(useCases.UpdateContact(new UpdateContactDto(
                user.GetUserId(), contactId,
                request.FirstName, request.LastName, request.Sex, request.Age))))
    .WithSummary("Modyfikuje kontakt")
    .WithDescription("Zmienia imię, nazwisko, płeć i wiek kontaktu.")
    .Produces(StatusCodes.Status204NoContent)
    .ProducesProblem(StatusCodes.Status400BadRequest);

contacts.MapDelete("/{contactId:guid}",
        (Guid contactId, IContactUseCases useCases, ClaimsPrincipal user) =>
            NoContent(useCases.RemoveContact(user.GetUserId(), contactId)))
    .WithSummary("Usuwa kontakt")
    .WithDescription("Usuwa kontakt wraz z jego adresami email, telefonami i datami.")
    .Produces(StatusCodes.Status204NoContent)
    .ProducesProblem(StatusCodes.Status400BadRequest);

contacts.MapPost("/{contactId:guid}/emails",
        (Guid contactId, AddEmailRequest request, IContactUseCases useCases, ClaimsPrincipal user) =>
            NoContent(useCases.AddEmail(new AddEmailDto(
                user.GetUserId(), contactId, request.Address, request.CategoryId))))
    .WithSummary("Dodaje adres email do kontaktu")
    .WithDescription("Dołącza do kontaktu adres email przypisany do jednej z kategorii użytkownika.")
    .Produces(StatusCodes.Status204NoContent)
    .ProducesProblem(StatusCodes.Status400BadRequest);

contacts.MapDelete("/{contactId:guid}/emails/{address}",
        (Guid contactId, string address, IContactUseCases useCases, ClaimsPrincipal user) =>
            NoContent(useCases.RemoveEmail(user.GetUserId(), contactId, address)))
    .WithSummary("Usuwa adres email z kontaktu")
    .WithDescription("Usuwa z kontaktu podany adres email.")
    .Produces(StatusCodes.Status204NoContent)
    .ProducesProblem(StatusCodes.Status400BadRequest);

contacts.MapPost("/{contactId:guid}/phones",
        (Guid contactId, PhoneRequest request, IContactUseCases useCases, ClaimsPrincipal user) =>
            NoContent(useCases.AddPhone(user.GetUserId(), contactId, request.Number)))
    .WithSummary("Dodaje numer telefonu do kontaktu")
    .WithDescription("Dołącza do kontaktu numer telefonu (9-15 cyfr, opcjonalnie z prefiksem +).")
    .Produces(StatusCodes.Status204NoContent)
    .ProducesProblem(StatusCodes.Status400BadRequest);

contacts.MapDelete("/{contactId:guid}/phones/{number}",
        (Guid contactId, string number, IContactUseCases useCases, ClaimsPrincipal user) =>
            NoContent(useCases.RemovePhone(user.GetUserId(), contactId, number)))
    .WithSummary("Usuwa numer telefonu z kontaktu")
    .WithDescription("Usuwa z kontaktu podany numer telefonu.")
    .Produces(StatusCodes.Status204NoContent)
    .ProducesProblem(StatusCodes.Status400BadRequest);

contacts.MapPost("/{contactId:guid}/important-dates",
        (Guid contactId, ImportantDateRequest request, IContactUseCases useCases, ClaimsPrincipal user) =>
            NoContent(useCases.AddImportantDate(
                user.GetUserId(), contactId, request.Date, request.Description)))
    .WithSummary("Dodaje ważną datę do kontaktu")
    .WithDescription("Dołącza do kontaktu datę z opisem (np. urodziny). Opis musi być unikalny w kontakcie.")
    .Produces(StatusCodes.Status204NoContent)
    .ProducesProblem(StatusCodes.Status400BadRequest);

contacts.MapDelete("/{contactId:guid}/important-dates/{description}",
        (Guid contactId, string description, IContactUseCases useCases, ClaimsPrincipal user) =>
            NoContent(useCases.RemoveImportantDate(user.GetUserId(), contactId, description)))
    .WithSummary("Usuwa ważną datę z kontaktu")
    .WithDescription("Usuwa z kontaktu datę o podanym opisie.")
    .Produces(StatusCodes.Status204NoContent)
    .ProducesProblem(StatusCodes.Status400BadRequest);

// Uruchomienie aplikacji
app.Run();

// Wynik komendy niezwracającej danych: 204 przy sukcesie, 400 z opisem błędu w przeciwnym razie.
static IResult NoContent(Result<Unit> result) =>
    result.Match(
        _ => Results.NoContent(),
        error => Results.Problem(detail: error, statusCode: 400));

// Zadanie 11 - autoryzacja oparta o zasoby: najpierw pobieramy kategorię,
// a następnie IAuthorizationService decyduje, czy użytkownik może na niej działać.
// 404 - kategoria nie istnieje, 403 - istnieje, ale należy do kogoś innego.
static async Task<IResult> AuthorizeCategory(
    Guid categoryId,
    ClaimsPrincipal user,
    ICategoryQueries queries,
    IAuthorizationService authorization,
    Func<IResult> action) =>
    await queries
        .FindCategory(categoryId)
        .Match(
            async category =>
                (await authorization.AuthorizeAsync(user, category, "CategoryOwner")).Succeeded
                    ? action()
                    : Results.Forbid(),
            () => Task.FromResult(Results.NotFound()));

using System.Security.Claims;
using System.Text;
using System.Text.Json.Serialization;
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
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Metadata;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using WebAPI;
using WebAPI.Authorization;
using WebAPI.Helpers;
using WebAPI.Inputs;

var builder = WebApplication.CreateBuilder(args);

// ======================= UWIERZYTELNIANIE I AUTORYZACJA =======================
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
        // Zadanie 10: token ważny 15 minut - bez domyślnej 5-minutowej tolerancji
        ClockSkew = TimeSpan.FromSeconds(30),
        NameClaimType = "name",
        RoleClaimType = "role"
    };
    x.MapInboundClaims = false;
});

// 3.38 + zadanie 11
builder.Services.AddAuthorizationBuilder()
    .AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"))
    .AddPolicy(OwnershipAuthorization.PolicyName, policy =>
        policy.AddRequirements(new OwnerOrAdminRequirement()));
builder.Services.AddSingleton<IAuthorizationHandler, OwnerOrAdminHandler>();

// ======================= USŁUGI =======================
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("default")));

// Repozytoria, zapytania i jednostka pracy
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<IContactRepository, ContactRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<ICategoryQueries, CategoryQueries>();
builder.Services.AddScoped<IContactQueries, ContactQueries>();
builder.Services.AddScoped<IUserQueries, UserQueries>();

// Infrastruktura (porty z Application.OutputPorts)
builder.Services.AddScoped<ITokenGenerator, TokenGenerator>();
builder.Services.AddSingleton<IPasswordHasher, IdentityPasswordHasher>();

// Usługi domenowe
builder.Services.AddScoped<CategoryUniquenessChecker>();
builder.Services.AddScoped<CategoryRemovalChecker>();
builder.Services.AddScoped<UserUniquenessChecker>();

// Przypadki użycia
builder.Services.AddScoped<ICategoryUseCases, CategoryServices>();
builder.Services.AddScoped<IContactUseCases, ContactServices>();
builder.Services.AddScoped<IUserUseCases, UserServices>();

// Enumy (Sex, PhoneType, UserRole) jako tekst w JSON-ie i w Swaggerze
builder.Services.ConfigureHttpJsonOptions(options =>
    options.SerializerOptions.Converters.Add(new JsonStringEnumConverter()));

builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

// 3.22: schemat Bearer w dokumencie OpenAPI -> przycisk "Authorize" w Swaggerze
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

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    // Baza zawsze w aktualnej wersji (migracje stosowane przy starcie)
    scope.ServiceProvider.GetRequiredService<AppDbContext>().Database.Migrate();

    // Zadanie 6: pierwszy administrator z konfiguracji (appsettings.Development.json),
    // bo bez niego nikt nie mógłby nadać roli Admin
    var adminName = app.Configuration["AdminAccount:UserName"];
    var adminPassword = app.Configuration["AdminAccount:Password"];
    if (!string.IsNullOrWhiteSpace(adminName) && !string.IsNullOrWhiteSpace(adminPassword))
    {
        scope.ServiceProvider.GetRequiredService<IUserUseCases>()
            .EnsureAdminExists(adminName, adminPassword)
            .Match(
                _ => Unit.Instance,
                error =>
                {
                    app.Logger.LogError("Nie udało się utworzyć administratora: {Error}", error);
                    return Unit.Instance;
                });
    }
}

app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUI(options =>
        options.SwaggerEndpoint("/openapi/v1.json", "API v1"));
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

// Kody wspólne dla wielu akcji, dokumentowane raz dla grupy
var internalServerError = new ProducesResponseTypeMetadata(
    StatusCodes.Status500InternalServerError,
    typeof(ProblemDetails),
    ["application/problem+json"]);
// 401 i 403 zwraca middleware uwierzytelniania (bez treści)
var unauthorized = new ProducesResponseTypeMetadata(StatusCodes.Status401Unauthorized);

// ======================= UWIERZYTELNIANIE =======================
var auth = app.MapGroup("/api")
    .WithTags("Uwierzytelnianie")
    .WithMetadata(internalServerError);

// 3.21: oba błędy (zły login, złe hasło) dają ten sam kod 401,
// żeby nie zdradzać, czy dany login istnieje
auth.MapPost("/login", (LoginUserDto dto, IUserUseCases userUseCases) =>
        userUseCases.Login(dto).Match(
            loginData => Results.Ok(loginData),
            _ => Results.Unauthorized()))
    .AllowAnonymous()
    .WithName("Login")
    .WithSummary("Loguje użytkownika")
    .WithDescription(
        "Zwraca token dostępowy (accessToken, ważny 15 minut) i token odświeżający " +
        "(refreshToken, ważny 7 dni). accessToken wklej w oknie \"Authorize\".")
    .Produces<LoginUserResult>(StatusCodes.Status200OK)
    .Produces(StatusCodes.Status401Unauthorized);

// Zadanie 10
auth.MapPost("/refresh", (RefreshTokenDto dto, IUserUseCases userUseCases) =>
        userUseCases.RefreshToken(dto).Match(
            loginData => Results.Ok(loginData),
            _ => Results.Unauthorized()))
    .AllowAnonymous()
    .WithName("RefreshToken")
    .WithSummary("Odświeża token dostępowy")
    .WithDescription(
        "Wymienia token odświeżający na nową parę tokenów bez podawania hasła. " +
        "Użyty token odświeżający przestaje działać.")
    .Produces<LoginUserResult>(StatusCodes.Status200OK)
    .Produces(StatusCodes.Status401Unauthorized);

auth.MapPost("/logout", (IUserUseCases userUseCases, ClaimsPrincipal user) =>
        userUseCases.Logout(user.GetUserId()).ToNoContent())
    .RequireAuthorization()
    .WithName("Logout")
    .WithSummary("Wylogowuje użytkownika")
    .WithDescription(
        "Unieważnia token odświeżający. Token dostępowy działa jeszcze do swojego wygaśnięcia.")
    .Produces(StatusCodes.Status204NoContent)
    .Produces(StatusCodes.Status401Unauthorized)
    .ProducesProblem(StatusCodes.Status404NotFound);

// ======================= UŻYTKOWNICY =======================
// Rejestracja jest jedyną akcją użytkowników dostępną bez logowania
app.MapPost("/api/users", (AddUserDto dto, IUserUseCases userUseCases) =>
        userUseCases
            .AddUser(dto)
            .ToHttpResult(user => Results.Created($"/api/users/{user.Id}", user)))
    .AllowAnonymous()
    .WithTags("Użytkownicy")
    .WithName("AddUser")
    .WithSummary("Rejestruje użytkownika")
    .WithDescription(
        "Tworzy użytkownika z rolą User. Login musi być unikalny, hasło musi mieć " +
        "8–100 znaków, w tym co najmniej jedną literę i jedną cyfrę.")
    .Produces<AddUserResult>(StatusCodes.Status201Created)
    .ProducesProblem(StatusCodes.Status400BadRequest)
    .ProducesProblem(StatusCodes.Status409Conflict)
    .WithMetadata(internalServerError);

var users = app.MapGroup("/api/users")
    .WithTags("Użytkownicy")
    .WithMetadata(internalServerError)
    .WithMetadata(unauthorized)
    .RequireAuthorization();

// Zadanie 5: lista użytkowników tylko dla administratora
users.MapGet("/", (IUserQueries queries) => Results.Ok(queries.GetUsers()))
    .RequireAuthorization("AdminOnly")
    .WithName("GetUsers")
    .WithSummary("Zwraca listę użytkowników (Admin)")
    .WithDescription("Zwraca wszystkich użytkowników razem z profilami. Tylko dla administratora.")
    .Produces<IEnumerable<UserDto>>(StatusCodes.Status200OK)
    .Produces(StatusCodes.Status403Forbidden);

users.MapGet("/me", (IUserQueries queries, ClaimsPrincipal user) =>
        queries.GetUser(user.GetUserId()).Match(
            found => Results.Ok(found),
            () => ResultHttpExtensions.NotFound("Brak użytkownika")))
    .WithName("GetCurrentUser")
    .WithSummary("Zwraca dane zalogowanego użytkownika")
    .WithDescription("Zwraca login, rolę i profil zalogowanego użytkownika.")
    .Produces<UserDto>(StatusCodes.Status200OK)
    .ProducesProblem(StatusCodes.Status404NotFound);

users.MapGet("/{id:guid}", (Guid id, IUserQueries queries) =>
        queries.GetUser(id).Match(
            found => Results.Ok(found),
            () => ResultHttpExtensions.NotFound($"Brak użytkownika (id: {id})")))
    .RequireAuthorization("AdminOnly")
    .WithName("GetUser")
    .WithSummary("Zwraca wskazanego użytkownika (Admin)")
    .WithDescription("Zwraca login, rolę i profil użytkownika. Tylko dla administratora.")
    .Produces<UserDto>(StatusCodes.Status200OK)
    .Produces(StatusCodes.Status403Forbidden)
    .ProducesProblem(StatusCodes.Status404NotFound);

// Zadania 4 i 5: każdy zalogowany edytuje tylko własny profil
users.MapPut("/me/profile",
        (UpdateProfileRequest request, IUserUseCases userUseCases, ClaimsPrincipal user) =>
            userUseCases.UpdateProfile(new UpdateProfileDto(
                    user.GetUserId(), request.FirstName, request.LastName,
                    request.BirthDate, request.Address))
                .ToNoContent())
    .WithName("UpdateProfile")
    .WithSummary("Zmienia profil zalogowanego użytkownika")
    .WithDescription(
        "Ustawia imię, nazwisko, datę urodzenia i adres. Wszystkie pola są opcjonalne; " +
        "puste pole czyści wartość.")
    .Produces(StatusCodes.Status204NoContent)
    .ProducesProblem(StatusCodes.Status400BadRequest)
    .ProducesProblem(StatusCodes.Status404NotFound);

// Zadanie 3
users.MapPut("/me/password",
        (ChangePasswordRequest request, IUserUseCases userUseCases, ClaimsPrincipal user) =>
            userUseCases.ChangePassword(new ChangePasswordDto(
                    user.GetUserId(), request.CurrentPassword, request.NewPassword))
                .ToNoContent())
    .WithName("ChangePassword")
    .WithSummary("Zmienia hasło zalogowanego użytkownika")
    .WithDescription(
        "Wymaga podania aktualnego hasła. Nowe hasło musi spełniać te same reguły co przy " +
        "rejestracji. Po zmianie token odświeżający przestaje działać.")
    .Produces(StatusCodes.Status204NoContent)
    .ProducesProblem(StatusCodes.Status400BadRequest)
    .ProducesProblem(StatusCodes.Status404NotFound);

// Zadanie 6
users.MapPut("/{id:guid}/role",
        (Guid id, ChangeRoleRequest request, IUserUseCases userUseCases, ClaimsPrincipal user) =>
            userUseCases.ChangeRole(new ChangeRoleDto(id, request.Role, user.GetUserId()))
                .ToNoContent())
    .RequireAuthorization("AdminOnly")
    .WithName("ChangeUserRole")
    .WithSummary("Zmienia rolę użytkownika (Admin)")
    .WithDescription(
        "Nadaje rolę User lub Admin. Administrator nie może zmienić własnej roli. " +
        "Nowa rola obowiązuje od następnego logowania lub odświeżenia tokenu.")
    .Produces(StatusCodes.Status204NoContent)
    .ProducesProblem(StatusCodes.Status400BadRequest)
    .Produces(StatusCodes.Status403Forbidden)
    .ProducesProblem(StatusCodes.Status404NotFound);

// Zadanie 5 (przykład 3.37/3.38 z PDF)
users.MapDelete("/{id:guid}", (Guid id, IUserUseCases userUseCases, ClaimsPrincipal user) =>
        userUseCases.RemoveUser(new RemoveUserDto(id, user.GetUserId())).ToNoContent())
    .RequireAuthorization("AdminOnly")
    .WithName("RemoveUser")
    .WithSummary("Usuwa użytkownika (Admin)")
    .WithDescription(
        "Usuwa użytkownika razem z jego kontaktami i kategoriami. " +
        "Administrator nie może usunąć własnego konta.")
    .Produces(StatusCodes.Status204NoContent)
    .ProducesProblem(StatusCodes.Status400BadRequest)
    .Produces(StatusCodes.Status403Forbidden)
    .ProducesProblem(StatusCodes.Status404NotFound);

// ======================= KATEGORIE =======================
var categories = app.MapGroup("/api/categories")
    .WithTags("Kategorie")
    .WithMetadata(internalServerError)
    .WithMetadata(unauthorized)
    .RequireAuthorization();

// 3.33: identyfikator użytkownika zawsze z tokenu, nigdy od klienta (ochrona przed IDOR)
categories.MapGet("/", (ICategoryQueries queries, ClaimsPrincipal user) =>
        Results.Ok(queries.GetCategories(user.GetUserId())))
    .WithName("GetCategories")
    .WithSummary("Zwraca kategorie zalogowanego użytkownika")
    .WithDescription("Zwraca listę kategorii zalogowanego użytkownika. Gdy nie ma żadnej, lista jest pusta.")
    .Produces<IEnumerable<CategoryDto>>(StatusCodes.Status200OK);

// Zadanie 9
categories.MapGet("/{id:guid}", (Guid id, ICategoryQueries queries, ClaimsPrincipal user) =>
        queries.GetCategory(id, user.GetUserId()).Match(
            category => Results.Ok(category),
            () => ResultHttpExtensions.NotFound(
                $"Nie mogę odnaleźć kategorii o id = {id}")))
    .WithName("GetCategory")
    .WithSummary("Zwraca jedną kategorię")
    .WithDescription("Zwraca kategorię o podanym identyfikatorze, jeżeli należy do zalogowanego użytkownika.")
    .Produces<CategoryDto>(StatusCodes.Status200OK)
    .ProducesProblem(StatusCodes.Status404NotFound);

// 3.29: nazwa z treści żądania, właściciel z tokenu
categories.MapPost("/", (AddCategoryRequest input, ICategoryUseCases useCases, ClaimsPrincipal user) =>
        useCases
            .AddCategory(new AddCategoryDto(input.Name, user.GetUserId()))
            .ToHttpResult(category =>
                Results.Created($"/api/categories/{category.Id}", category)))
    .WithName("AddCategory")
    .WithSummary("Dodaje kategorię")
    .WithDescription(
        "Tworzy nową kategorię zalogowanego użytkownika. Nazwa nie może być pusta " +
        "i musi być unikalna wśród jego kategorii.")
    .Produces<AddCategoryResult>(StatusCodes.Status201Created)
    .ProducesProblem(StatusCodes.Status400BadRequest)
    .ProducesProblem(StatusCodes.Status409Conflict);

// Zadanie 11: zmienić kategorię może tylko jej właściciel lub administrator
categories.MapPut("/{id:guid}",
        (Guid id, RenameCategoryRequest request, ICategoryQueries queries,
            ICategoryUseCases useCases, IAuthorizationService authorization, ClaimsPrincipal user) =>
            authorization.AuthorizeOwnerAsync(
                user,
                queries.GetCategoryOwner(id),
                $"Nie mogę odnaleźć kategorii o id = {id}",
                () => useCases.RenameCategory(new RenameCategoryDto(id, request.Name)).ToNoContent()))
    .WithName("RenameCategory")
    .WithSummary("Zmienia nazwę kategorii")
    .WithDescription(
        "Zmienia nazwę kategorii. Dozwolone dla właściciela kategorii i administratora. " +
        "Nowa nazwa nie może być pusta ani zajęta przez inną kategorię właściciela.")
    .Produces(StatusCodes.Status204NoContent)
    .ProducesProblem(StatusCodes.Status400BadRequest)
    .Produces(StatusCodes.Status403Forbidden)
    .ProducesProblem(StatusCodes.Status404NotFound)
    .ProducesProblem(StatusCodes.Status409Conflict);

// Zadanie 11
categories.MapDelete("/{id:guid}",
        (Guid id, ICategoryQueries queries, ICategoryUseCases useCases,
            IAuthorizationService authorization, ClaimsPrincipal user) =>
            authorization.AuthorizeOwnerAsync(
                user,
                queries.GetCategoryOwner(id),
                $"Nie mogę odnaleźć kategorii o id = {id}",
                () => useCases.RemoveCategory(id).ToNoContent()))
    .WithName("RemoveCategory")
    .WithSummary("Usuwa kategorię")
    .WithDescription(
        "Usuwa kategorię. Dozwolone dla właściciela kategorii i administratora. " +
        "Kategorii przypisanej do jakiegokolwiek adresu e-mail nie można usunąć (409).")
    .Produces(StatusCodes.Status204NoContent)
    .Produces(StatusCodes.Status403Forbidden)
    .ProducesProblem(StatusCodes.Status404NotFound)
    .ProducesProblem(StatusCodes.Status409Conflict);

// ======================= KONTAKTY =======================
// Zadania 1 i 8: kontakty wymagają logowania i należą do zalogowanego użytkownika.
// Cudzy kontakt zachowuje się tak, jakby nie istniał (404).
var contacts = app.MapGroup("/api/contacts")
    .WithTags("Kontakty")
    .WithMetadata(internalServerError)
    .WithMetadata(unauthorized)
    .RequireAuthorization();

contacts.MapGet("/", (IContactQueries queries, ClaimsPrincipal user) =>
        Results.Ok(queries.GetContacts(user.GetUserId())))
    .WithName("GetContacts")
    .WithSummary("Zwraca kontakty zalogowanego użytkownika")
    .WithDescription("Zwraca listę kontaktów (bez e-maili, telefonów i dat).")
    .Produces<IEnumerable<ContactSummaryDto>>(StatusCodes.Status200OK);

contacts.MapGet("/{id:guid}", (Guid id, IContactQueries queries, ClaimsPrincipal user) =>
        queries.GetContact(id, user.GetUserId()).Match(
            contact => Results.Ok(contact),
            () => ResultHttpExtensions.NotFound($"Brak kontaktu (id: {id})")))
    .WithName("GetContact")
    .WithSummary("Zwraca jeden kontakt")
    .WithDescription("Zwraca kontakt razem z adresami e-mail, numerami telefonów i ważnymi datami.")
    .Produces<ContactDetailsDto>(StatusCodes.Status200OK)
    .ProducesProblem(StatusCodes.Status404NotFound);

contacts.MapPost("/", (AddContactRequest request, IContactUseCases useCases, ClaimsPrincipal user) =>
        useCases.AddContact(new AddContactDto(
                request.FirstName, request.LastName, request.Sex, request.Age, user.GetUserId()))
            .ToHttpResult(contact =>
                Results.Created($"/api/contacts/{contact.Id}", contact)))
    .WithName("AddContact")
    .WithSummary("Dodaje kontakt")
    .WithDescription(
        "Tworzy nowy kontakt zalogowanego użytkownika. Imię i nazwisko są wymagane " +
        "(maks. 50 znaków), wiek musi mieścić się w dozwolonym zakresie.")
    .Produces<AddContactResult>(StatusCodes.Status201Created)
    .ProducesProblem(StatusCodes.Status400BadRequest);

contacts.MapPut("/{id:guid}",
        (Guid id, UpdateContactRequest request, IContactUseCases useCases, ClaimsPrincipal user) =>
            useCases.UpdateContact(new UpdateContactDto(
                    id, request.FirstName, request.LastName, request.Sex, request.Age,
                    user.GetUserId()))
                .ToNoContent())
    .WithName("UpdateContact")
    .WithSummary("Edytuje kontakt")
    .WithDescription(
        "Zmienia imię, nazwisko, płeć i wiek kontaktu. " +
        "Gdy któraś wartość jest niepoprawna, kontakt nie zmienia się wcale.")
    .Produces(StatusCodes.Status204NoContent)
    .ProducesProblem(StatusCodes.Status400BadRequest)
    .ProducesProblem(StatusCodes.Status404NotFound);

contacts.MapDelete("/{id:guid}", (Guid id, IContactUseCases useCases, ClaimsPrincipal user) =>
        useCases.RemoveContact(id, user.GetUserId()).ToNoContent())
    .WithName("RemoveContact")
    .WithSummary("Usuwa kontakt")
    .WithDescription("Usuwa kontakt razem z jego adresami e-mail, telefonami i datami.")
    .Produces(StatusCodes.Status204NoContent)
    .ProducesProblem(StatusCodes.Status404NotFound);

// ---------- Kontakty: E-maile ----------

contacts.MapPost("/{id:guid}/emails",
        (Guid id, AddEmailRequest request, IContactUseCases useCases, ClaimsPrincipal user) =>
            useCases.AddEmail(new AddEmailDto(id, request.Address, request.CategoryId, user.GetUserId()))
                .ToNoContent())
    .WithName("AddEmail")
    .WithSummary("Dodaje adres e-mail do kontaktu")
    .WithDescription(
        "Dołącza adres e-mail z podaną kategorią. Adres musi mieć poprawny format, " +
        "kategoria musi należeć do zalogowanego użytkownika, a kontakt nie może mieć już tego adresu.")
    .Produces(StatusCodes.Status204NoContent)
    .ProducesProblem(StatusCodes.Status400BadRequest)
    .ProducesProblem(StatusCodes.Status404NotFound)
    .ProducesProblem(StatusCodes.Status409Conflict);

contacts.MapDelete("/{id:guid}/emails/{address}",
        (Guid id, string address, IContactUseCases useCases, ClaimsPrincipal user) =>
            useCases.RemoveEmail(new RemoveEmailDto(id, address, user.GetUserId())).ToNoContent())
    .WithName("RemoveEmail")
    .WithSummary("Usuwa adres e-mail z kontaktu")
    .WithDescription("Usuwa podany adres e-mail. Wielkość liter w adresie nie ma znaczenia.")
    .Produces(StatusCodes.Status204NoContent)
    .ProducesProblem(StatusCodes.Status400BadRequest)
    .ProducesProblem(StatusCodes.Status404NotFound);

// ---------- Kontakty: Telefony ----------

contacts.MapPost("/{id:guid}/phones",
        (Guid id, AddPhoneRequest request, IContactUseCases useCases, ClaimsPrincipal user) =>
            useCases.AddPhone(new AddPhoneDto(id, request.Number, request.Type, user.GetUserId()))
                .ToNoContent())
    .WithName("AddPhone")
    .WithSummary("Dodaje numer telefonu do kontaktu")
    .WithDescription(
        "Dołącza numer telefonu (9–15 cyfr, opcjonalnie z \"+\"; spacje, myślniki " +
        "i nawiasy są pomijane). Ten sam numer nie może wystąpić w kontakcie dwa razy.")
    .Produces(StatusCodes.Status204NoContent)
    .ProducesProblem(StatusCodes.Status400BadRequest)
    .ProducesProblem(StatusCodes.Status404NotFound)
    .ProducesProblem(StatusCodes.Status409Conflict);

contacts.MapDelete("/{id:guid}/phones/{number}",
        (Guid id, string number, IContactUseCases useCases, ClaimsPrincipal user) =>
            useCases.RemovePhone(new RemovePhoneDto(id, number, user.GetUserId())).ToNoContent())
    .WithName("RemovePhone")
    .WithSummary("Usuwa numer telefonu z kontaktu")
    .WithDescription("Usuwa podany numer telefonu z kontaktu.")
    .Produces(StatusCodes.Status204NoContent)
    .ProducesProblem(StatusCodes.Status400BadRequest)
    .ProducesProblem(StatusCodes.Status404NotFound);

// ---------- Kontakty: Ważne daty ----------

contacts.MapPost("/{id:guid}/dates",
        (Guid id, ImportantDateRequest request, IContactUseCases useCases, ClaimsPrincipal user) =>
            useCases.AddImportantDate(new AddImportantDateDto(id, request.Label, request.Date, user.GetUserId()))
                .ToHttpResult(dateId => Results.Created(
                    $"/api/contacts/{id}", new CreatedIdResponse(dateId))))
    .WithName("AddImportantDate")
    .WithSummary("Dodaje ważną datę do kontaktu")
    .WithDescription(
        "Dodaje datę z etykietą (np. \"Urodziny\") i zwraca jej identyfikator. " +
        "Etykiety w obrębie kontaktu są unikalne.")
    .Produces<CreatedIdResponse>(StatusCodes.Status201Created)
    .ProducesProblem(StatusCodes.Status400BadRequest)
    .ProducesProblem(StatusCodes.Status404NotFound)
    .ProducesProblem(StatusCodes.Status409Conflict);

contacts.MapPut("/{id:guid}/dates/{dateId:guid}",
        (Guid id, Guid dateId, ImportantDateRequest request, IContactUseCases useCases, ClaimsPrincipal user) =>
            useCases.ChangeImportantDate(new ChangeImportantDateDto(
                    id, dateId, request.Label, request.Date, user.GetUserId()))
                .ToNoContent())
    .WithName("ChangeImportantDate")
    .WithSummary("Zmienia ważną datę kontaktu")
    .WithDescription("Zmienia etykietę i datę. Nowa etykieta nie może być zajęta przez inną datę kontaktu.")
    .Produces(StatusCodes.Status204NoContent)
    .ProducesProblem(StatusCodes.Status400BadRequest)
    .ProducesProblem(StatusCodes.Status404NotFound)
    .ProducesProblem(StatusCodes.Status409Conflict);

contacts.MapDelete("/{id:guid}/dates/{dateId:guid}",
        (Guid id, Guid dateId, IContactUseCases useCases, ClaimsPrincipal user) =>
            useCases.RemoveImportantDate(new RemoveImportantDateDto(id, dateId, user.GetUserId()))
                .ToNoContent())
    .WithName("RemoveImportantDate")
    .WithSummary("Usuwa ważną datę z kontaktu")
    .WithDescription("Usuwa datę o podanym identyfikatorze.")
    .Produces(StatusCodes.Status204NoContent)
    .ProducesProblem(StatusCodes.Status404NotFound);

app.Run();

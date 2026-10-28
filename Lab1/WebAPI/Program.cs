using System.Text.Json.Serialization;
using Application.DomainServices;
using Application.OutputPorts;
using Application.Queries;
using Application.Queries.Dtos;
using Application.Repositories;
using Application.Services;
using Application.UseCases;
using Application.UseCases.Dtos;
using Data;
using Microsoft.AspNetCore.Http.Metadata;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebAPI;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Application.OutputPorts;
using WebAPI.Helpers;

var builder = WebApplication.CreateBuilder(args);

// Uwierzytelnianie JWT
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

// Kontekst danych
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("default")));

// Repozytoria, zapytania i jednostka pracy
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<IContactRepository, ContactRepository>();
builder.Services.AddScoped<ICategoryQueries, CategoryQueries>();
builder.Services.AddScoped<IContactQueries, ContactQueries>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUserUseCases, UserServices>();
builder.Services.AddScoped<ITokenGenerator, TokenGenerator>();
// Usługi domenowe
builder.Services.AddScoped<CategoryUniquenessChecker>();
builder.Services.AddScoped<CategoryRemovalChecker>();

// Przypadki użycia
builder.Services.AddScoped<ICategoryUseCases, CategoryServices>();
builder.Services.AddScoped<IContactUseCases, ContactServices>();

// Enumy (Sex, PhoneType) jako tekst w JSON-ie i w Swaggerze, np. "Male" zamiast 0
builder.Services.ConfigureHttpJsonOptions(options =>
    options.SerializerOptions.Converters.Add(new JsonStringEnumConverter()));

builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddOpenApi();

var app = builder.Build();

// Baza zawsze w aktualnej wersji (tabele Phone i ImportantDate z Lab1)
using (var scope = app.Services.CreateScope())
{
    scope.ServiceProvider.GetRequiredService<AppDbContext>().Database.Migrate();
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

// Kod 500 (GlobalExceptionHandler) może zwrócić każda akcja, więc dokumentujemy go raz dla grupy.
// ProducesProblem działa tylko na pojedynczej akcji, dlatego metadane dodajemy ręcznie.
var internalServerError = new ProducesResponseTypeMetadata(
    StatusCodes.Status500InternalServerError,
    typeof(ProblemDetails),
    ["application/problem+json"]);

// ======================= KATEGORIE =======================
// Zadanie 4: wspólny prefiks ścieżki i wspólny tag dla wszystkich akcji grupy
var categories = app.MapGroup("/api/categories")
    .WithTags("Kategorie")
    .WithMetadata(internalServerError)
    .RequireAuthorization();

categories.MapGet("/", (ICategoryQueries queries) =>
        Results.Ok(queries.GetCategories()))
    .WithName("GetCategories")
    .WithSummary("Zwraca wszystkie kategorie")
    .WithDescription("Zwraca listę wszystkich kategorii. Gdy nie ma żadnej, lista jest pusta.")
    .Produces<IEnumerable<CategoryDto>>(StatusCodes.Status200OK);

categories.MapGet("/{id:guid}", (Guid id, ICategoryQueries queries) =>
        queries.GetCategory(id).Match(
            category => Results.Ok(category),
            () => ResultHttpExtensions.NotFound(
                $"Nie mogę odnaleźć kategorii o id = {id}")))
    .WithName("GetCategory")
    .WithSummary("Zwraca jedną kategorię")
    .WithDescription("Zwraca kategorię o podanym identyfikatorze.")
    .Produces<CategoryDto>(StatusCodes.Status200OK)
    .ProducesProblem(StatusCodes.Status404NotFound);

categories.MapPost("/", (AddCategoryDto dto, ICategoryUseCases useCases) =>
        useCases.AddCategory(dto).ToHttpResult(category =>
            Results.Created($"/api/categories/{category.Id}", category)))
    .WithName("AddCategory")
    .WithSummary("Dodaje kategorię")
    .WithDescription(
        "Tworzy nową kategorię i zwraca utworzony obiekt. " +
        "Nazwa nie może być pusta i musi być unikalna.")
    .Produces<AddCategoryResult>(StatusCodes.Status201Created)
    .ProducesProblem(StatusCodes.Status400BadRequest)
    .ProducesProblem(StatusCodes.Status409Conflict);

categories.MapPut("/{id:guid}", (Guid id, RenameCategoryRequest request, ICategoryUseCases useCases) =>
        useCases.RenameCategory(new RenameCategoryDto(id, request.Name)).ToNoContent())
    .WithName("RenameCategory")
    .WithSummary("Zmienia nazwę kategorii")
    .WithDescription(
        "Zmienia nazwę istniejącej kategorii. Nowa nazwa nie może być pusta " +
        "i nie może być zajęta przez inną kategorię.")
    .Produces(StatusCodes.Status204NoContent)
    .ProducesProblem(StatusCodes.Status400BadRequest)
    .ProducesProblem(StatusCodes.Status404NotFound)
    .ProducesProblem(StatusCodes.Status409Conflict);

categories.MapDelete("/{id:guid}", (Guid id, ICategoryUseCases useCases) =>
        useCases.RemoveCategory(id).ToNoContent())
    .WithName("RemoveCategory")
    .WithSummary("Usuwa kategorię")
    .WithDescription(
        "Usuwa kategorię. Kategorii przypisanej do jakiegokolwiek adresu e-mail " +
        "nie można usunąć (409).")
    .Produces(StatusCodes.Status204NoContent)
    .ProducesProblem(StatusCodes.Status404NotFound)
    .ProducesProblem(StatusCodes.Status409Conflict);

// ======================= KONTAKTY =======================
var contacts = app.MapGroup("/api/contacts")
    .WithTags("Kontakty")
    .WithMetadata(internalServerError);

contacts.MapGet("/", (IContactQueries queries) =>
        Results.Ok(queries.GetContacts()))
    .WithName("GetContacts")
    .WithSummary("Zwraca wszystkie kontakty")
    .WithDescription("Zwraca listę kontaktów (bez e-maili, telefonów i dat).")
    .Produces<IEnumerable<ContactSummaryDto>>(StatusCodes.Status200OK);

contacts.MapGet("/{id:guid}", (Guid id, IContactQueries queries) =>
        queries.GetContact(id).Match(
            contact => Results.Ok(contact),
            () => ResultHttpExtensions.NotFound($"Brak kontaktu (id: {id})")))
    .WithName("GetContact")
    .WithSummary("Zwraca jeden kontakt")
    .WithDescription("Zwraca kontakt razem z adresami e-mail, numerami telefonów i ważnymi datami.")
    .Produces<ContactDetailsDto>(StatusCodes.Status200OK)
    .ProducesProblem(StatusCodes.Status404NotFound);

contacts.MapPost("/", (AddContactDto dto, IContactUseCases useCases) =>
        useCases.AddContact(dto).ToHttpResult(contact =>
            Results.Created($"/api/contacts/{contact.Id}", contact)))
    .WithName("AddContact")
    .WithSummary("Dodaje kontakt")
    .WithDescription(
        "Tworzy nowy kontakt. Imię i nazwisko są wymagane (maks. 50 znaków), " +
        "wiek musi mieścić się w dozwolonym zakresie.")
    .Produces<AddContactResult>(StatusCodes.Status201Created)
    .ProducesProblem(StatusCodes.Status400BadRequest);

contacts.MapPut("/{id:guid}", (Guid id, UpdateContactRequest request, IContactUseCases useCases) =>
        useCases.UpdateContact(new UpdateContactDto(
                id, request.FirstName, request.LastName, request.Sex, request.Age))
            .ToNoContent())
    .WithName("UpdateContact")
    .WithSummary("Edytuje kontakt")
    .WithDescription(
        "Zmienia imię, nazwisko, płeć i wiek kontaktu. " +
        "Gdy któraś wartość jest niepoprawna, kontakt nie zmienia się wcale.")
    .Produces(StatusCodes.Status204NoContent)
    .ProducesProblem(StatusCodes.Status400BadRequest)
    .ProducesProblem(StatusCodes.Status404NotFound);

contacts.MapDelete("/{id:guid}", (Guid id, IContactUseCases useCases) =>
        useCases.RemoveContact(id).ToNoContent())
    .WithName("RemoveContact")
    .WithSummary("Usuwa kontakt")
    .WithDescription("Usuwa kontakt razem z jego adresami e-mail, telefonami i datami.")
    .Produces(StatusCodes.Status204NoContent)
    .ProducesProblem(StatusCodes.Status404NotFound);

// ---------- Kontakty: E-maile ----------

contacts.MapPost("/{id:guid}/emails", (Guid id, AddEmailRequest request, IContactUseCases useCases) =>
        useCases.AddEmail(new AddEmailDto(id, request.Address, request.CategoryId))
            .ToNoContent())
    .WithName("AddEmail")
    .WithSummary("Dodaje adres e-mail do kontaktu")
    .WithDescription(
        "Dołącza adres e-mail z podaną kategorią. Adres musi mieć poprawny format, " +
        "kategoria musi istnieć, a kontakt nie może mieć już tego adresu.")
    .Produces(StatusCodes.Status204NoContent)
    .ProducesProblem(StatusCodes.Status400BadRequest)
    .ProducesProblem(StatusCodes.Status404NotFound)
    .ProducesProblem(StatusCodes.Status409Conflict);

contacts.MapDelete("/{id:guid}/emails/{address}", (Guid id, string address, IContactUseCases useCases) =>
        useCases.RemoveEmail(new RemoveEmailDto(id, address)).ToNoContent())
    .WithName("RemoveEmail")
    .WithSummary("Usuwa adres e-mail z kontaktu")
    .WithDescription("Usuwa podany adres e-mail. Wielkość liter w adresie nie ma znaczenia.")
    .Produces(StatusCodes.Status204NoContent)
    .ProducesProblem(StatusCodes.Status400BadRequest)
    .ProducesProblem(StatusCodes.Status404NotFound);

// ---------- Kontakty: Telefony ----------

contacts.MapPost("/{id:guid}/phones", (Guid id, AddPhoneRequest request, IContactUseCases useCases) =>
        useCases.AddPhone(new AddPhoneDto(id, request.Number, request.Type)).ToNoContent())
    .WithName("AddPhone")
    .WithSummary("Dodaje numer telefonu do kontaktu")
    .WithDescription(
        "Dołącza numer telefonu (9–15 cyfr, opcjonalnie z \"+\"; spacje, myślniki " +
        "i nawiasy są pomijane). Ten sam numer nie może wystąpić w kontakcie dwa razy.")
    .Produces(StatusCodes.Status204NoContent)
    .ProducesProblem(StatusCodes.Status400BadRequest)
    .ProducesProblem(StatusCodes.Status404NotFound)
    .ProducesProblem(StatusCodes.Status409Conflict);

contacts.MapDelete("/{id:guid}/phones/{number}", (Guid id, string number, IContactUseCases useCases) =>
        useCases.RemovePhone(new RemovePhoneDto(id, number)).ToNoContent())
    .WithName("RemovePhone")
    .WithSummary("Usuwa numer telefonu z kontaktu")
    .WithDescription("Usuwa podany numer telefonu z kontaktu.")
    .Produces(StatusCodes.Status204NoContent)
    .ProducesProblem(StatusCodes.Status400BadRequest)
    .ProducesProblem(StatusCodes.Status404NotFound);

// ---------- Kontakty: Ważne daty ----------

contacts.MapPost("/{id:guid}/dates", (Guid id, ImportantDateRequest request, IContactUseCases useCases) =>
        useCases.AddImportantDate(new AddImportantDateDto(id, request.Label, request.Date))
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
        (Guid id, Guid dateId, ImportantDateRequest request, IContactUseCases useCases) =>
            useCases.ChangeImportantDate(
                    new ChangeImportantDateDto(id, dateId, request.Label, request.Date))
                .ToNoContent())
    .WithName("ChangeImportantDate")
    .WithSummary("Zmienia ważną datę kontaktu")
    .WithDescription("Zmienia etykietę i datę. Nowa etykieta nie może być zajęta przez inną datę kontaktu.")
    .Produces(StatusCodes.Status204NoContent)
    .ProducesProblem(StatusCodes.Status400BadRequest)
    .ProducesProblem(StatusCodes.Status404NotFound)
    .ProducesProblem(StatusCodes.Status409Conflict);

contacts.MapDelete("/{id:guid}/dates/{dateId:guid}", (Guid id, Guid dateId, IContactUseCases useCases) =>
        useCases.RemoveImportantDate(new RemoveImportantDateDto(id, dateId)).ToNoContent())
    .WithName("RemoveImportantDate")
    .WithSummary("Usuwa ważną datę z kontaktu")
    .WithDescription("Usuwa datę o podanym identyfikatorze.")
    .Produces(StatusCodes.Status204NoContent)
    .ProducesProblem(StatusCodes.Status404NotFound);

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
loginData => Results.Ok(new loginData.AccessToken),
_ => Results.Unauthorized()
)
);



app.Run();

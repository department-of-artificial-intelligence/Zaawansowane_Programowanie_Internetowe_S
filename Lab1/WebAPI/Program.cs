using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Application.Domain;
using Application.DomainServices;
using Application.OutputPorts;
using Application.Queries;
using Application.Queries.Dtos;
using Application.Repositories;
using Application.Services;
using Application.UseCases;
using Application.UseCases.Dtos;
using Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using WebAPI.Helpers;
using WebAPI.Inputs;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(x =>
    {
        x.TokenValidationParameters = new TokenValidationParameters
        {
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["ApplicationSettings:JWT_KEY"]!)),
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true, 
            NameClaimType = "name",
            RoleClaimType = "role"
        };
        x.MapInboundClaims = false;
    });

builder.Services.AddAuthorizationBuilder()
    .AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"))
    .AddPolicy("CategoryOwnerOrAdmin", policy => policy.Requirements.Add(new CategoryOwnerOrAdminRequirement()));

builder.Services.AddSingleton<IAuthorizationHandler, CategoryAuthorizationHandler>();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("default"))
);

builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<ITokenGenerator, TokenGenerator>();
builder.Services.AddScoped<IPasswordHasher, PasswordHasher>();

builder.Services.AddScoped<ICategoryQueries, CategoryQueries>();
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<ICategoryUseCases, CategoryServices>();
builder.Services.AddScoped<CategoryUniquenessChecker>();

builder.Services.AddScoped<IContactQueries, ContactQueries>();
builder.Services.AddScoped<IContactRepository, ContactRepository>();
builder.Services.AddScoped<IContactUseCases, ContactServices>();

builder.Services.AddScoped<IUserQueries, UserQueries>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUserUseCases, UserServices>();
builder.Services.AddScoped<UserUniquenessChecker>();

builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<WebAPI.GlobalExceptionHandler>();

builder.Services.AddOpenApi(options =>
{
    options.AddDocumentTransformer((document, context, ct) =>
    {
        document.Components ??= new OpenApiComponents();
        document.Components.SecuritySchemes ??= new Dictionary<string, IOpenApiSecurityScheme>();
        document.Components.SecuritySchemes["Bearer"] = new OpenApiSecurityScheme
        {
            Type = SecuritySchemeType.Http,
            Scheme = "bearer",
            BearerFormat = "JWT",
            Description = "Wklej token JWT otrzymany z /api/auth/login"
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

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/openapi/v1.json", "API v1");
    });
}

app.UseExceptionHandler();
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

var categoriesGroup = app.MapGroup("/api/categories")
    .WithTags("Kategorie")
    .RequireAuthorization();

categoriesGroup.MapGet("", (ICategoryQueries queries, ClaimsPrincipal user) =>
    Results.Ok(queries.GetCategories(Guid.Parse(user.FindFirstValue(JwtRegisteredClaimNames.Sub)!)))
)
.WithSummary("Pobiera listę kategorii")
.WithDescription("Zwraca kolekcję kategorii należących do zalogowanego użytkownika.")
.Produces<IEnumerable<CategoryDto>>(StatusCodes.Status200OK);

categoriesGroup.MapGet("/{id:guid}", (Guid id, ICategoryQueries queries, ClaimsPrincipal user) =>
{
    var userId = Guid.Parse(user.FindFirstValue(JwtRegisteredClaimNames.Sub)!);
    return queries.GetCategory(id, userId).Match(
        category => Results.Ok(category),
        () => Results.NotFound()
    );
})
.WithSummary("Pobiera kategorię po ID")
.WithDescription("Zwraca szczegóły pojedynczej kategorii należącej do zalogowanego użytkownika.")
.Produces<CategoryDto>(StatusCodes.Status200OK)
.Produces(StatusCodes.Status404NotFound);

categoriesGroup.MapPost("", (AddCategoryRequest input, ICategoryUseCases useCases, ClaimsPrincipal user) =>
    useCases.AddCategory(new AddCategoryDto(
        input.Name,
        Guid.Parse(user.FindFirstValue(JwtRegisteredClaimNames.Sub)!))
    ).Match(
        category => Results.Created($"/api/categories/{category.Id}", category),
        error => Results.BadRequest(error)
    )
)
.WithSummary("Dodaje kategorię")
.WithDescription("Tworzy nową kategorię dla zalogowanego użytkownika.")
.Produces<AddCategoryResult>(StatusCodes.Status201Created)
.Produces(StatusCodes.Status400BadRequest);

categoriesGroup.MapPut("", async (
    UpdateCategoryDto dto,
    ICategoryRepository categoryRepository,
    ICategoryUseCases useCases,
    IAuthorizationService authService,
    ClaimsPrincipal user) =>
{
    var category = categoryRepository.GetById(dto.Id).Match<Category?>(c => c, () => null);
    if (category is null)
        return Results.NotFound();

    var authResult = await authService.AuthorizeAsync(user, category, "CategoryOwnerOrAdmin");
    if (!authResult.Succeeded)
        return Results.Forbid();

    return useCases.UpdateCategory(dto).Match(
        _ => Results.NoContent(),
        error => Results.Problem(detail: error, statusCode: StatusCodes.Status400BadRequest)
    );
})
.WithSummary("Aktualizuje kategorię")
.WithDescription("Modyfikuje kategorię po zweryfikowaniu uprawnień właściciela lub administratora.")
.Produces(StatusCodes.Status204NoContent)
.Produces(StatusCodes.Status400BadRequest)
.Produces(StatusCodes.Status403Forbidden)
.Produces(StatusCodes.Status404NotFound);

categoriesGroup.MapDelete("/{id:guid}", async (
    Guid id,
    ICategoryRepository categoryRepository,
    ICategoryUseCases useCases,
    IAuthorizationService authService,
    ClaimsPrincipal user) =>
{
    var category = categoryRepository.GetById(id).Match<Category?>(c => c, () => null);
    if (category is null)
        return Results.NotFound();

    var authResult = await authService.AuthorizeAsync(user, category, "CategoryOwnerOrAdmin");
    if (!authResult.Succeeded)
        return Results.Forbid();

    return useCases.RemoveCategory(id).Match(
        _ => Results.NoContent(),
        error => Results.Problem(detail: error, statusCode: StatusCodes.Status400BadRequest)
    );
})
.WithSummary("Usuwa kategorię")
.WithDescription("Usuwa kategorię po zweryfikowaniu uprawnień właściciela lub administratora.")
.Produces(StatusCodes.Status204NoContent)
.Produces(StatusCodes.Status400BadRequest)
.Produces(StatusCodes.Status403Forbidden)
.Produces(StatusCodes.Status404NotFound);

var contactGroup = app.MapGroup("/api/contacts")
    .WithTags("Kontakty")
    .RequireAuthorization();

contactGroup.MapGet("", (IContactQueries queries) =>
    Results.Ok(queries.GetContacts())
)
.WithSummary("Pobiera listę kontaktów")
.WithDescription("Zwraca kolekcję wszystkich dostępnych kontaktów w systemie wraz z ich szczegółami.")
.Produces<IEnumerable<ContactDto>>(StatusCodes.Status200OK);

contactGroup.MapGet("/{id:guid}", (Guid id, IContactQueries queries) =>
    queries.GetContact(id).Match(
        contact => Results.Ok(contact),
        () => Results.NotFound()
    )
)
.WithSummary("Pobiera kontakt po ID")
.WithDescription("Zwraca szczegóły pojedynczego kontaktu na podstawie jego unikalnego identyfikatora.")
.Produces<ContactDto>(StatusCodes.Status200OK)
.Produces(StatusCodes.Status404NotFound);

contactGroup.MapPost("", (AddContactDto dto, IContactUseCases useCases) =>
    useCases.AddContact(dto).Match(
        contact => Results.Created($"/api/contacts/{contact.Id}", contact),
        error => Results.Problem(detail: error, statusCode: StatusCodes.Status400BadRequest)
    )
)
.WithSummary("Dodaje kontakt")
.WithDescription("Tworzy nowy kontakt i zwraca utworzony obiekt.")
.Produces<AddContactResult>(StatusCodes.Status201Created)
.Produces(StatusCodes.Status400BadRequest);

contactGroup.MapPut("", (UpdateContactDto dto, IContactUseCases useCases) =>
    useCases.UpdateContact(dto).Match(
        _ => Results.NoContent(),
        error => Results.Problem(detail: error, statusCode: StatusCodes.Status400BadRequest)
    )
)
.WithSummary("Aktualizuje kontakt")
.WithDescription("Modyfikuje dane istniejącego kontaktu.")
.Produces(StatusCodes.Status204NoContent)
.Produces(StatusCodes.Status400BadRequest);

contactGroup.MapDelete("/{id:guid}", (Guid id, IContactUseCases useCases) =>
    useCases.RemoveContact(id).Match(
        _ => Results.NoContent(),
        error => Results.Problem(detail: error, statusCode: StatusCodes.Status400BadRequest)
    )
)
.WithSummary("Usuwa kontakt")
.WithDescription("Usuwa kontakt na podstawie identyfikatora.")
.Produces(StatusCodes.Status204NoContent)
.Produces(StatusCodes.Status400BadRequest);

contactGroup.MapPost("/emails", (AddEmailDto dto, IContactUseCases useCases) =>
    useCases.AddEmail(dto).Match(
        _ => Results.NoContent(),
        error => Results.Problem(detail: error, statusCode: StatusCodes.Status400BadRequest)
    )
)
.WithSummary("Dodaje e-mail do kontaktu")
.WithDescription("Przypisuje nowy adres e-mail do wskazanego kontaktu.")
.Produces(StatusCodes.Status204NoContent)
.Produces(StatusCodes.Status400BadRequest);

contactGroup.MapDelete("/emails", ([FromBody] RemoveEmailDto dto, IContactUseCases useCases) =>
    useCases.RemoveEmail(dto).Match(
        _ => Results.NoContent(),
        error => Results.Problem(detail: error, statusCode: StatusCodes.Status400BadRequest)
    )
)
.WithSummary("Usuwa e-mail z kontaktu")
.WithDescription("Usuwa powiązany adres e-mail z kontaktu.")
.Produces(StatusCodes.Status204NoContent)
.Produces(StatusCodes.Status400BadRequest);

contactGroup.MapPost("/phones", (AddPhoneDto dto, IContactUseCases useCases) =>
    useCases.AddPhone(dto).Match(
        _ => Results.NoContent(),
        error => Results.Problem(detail: error, statusCode: StatusCodes.Status400BadRequest)
    )
)
.WithSummary("Dodaje telefon do kontaktu")
.WithDescription("Przypisuje nowy numer telefonu do wskazanego kontaktu.")
.Produces(StatusCodes.Status204NoContent)
.Produces(StatusCodes.Status400BadRequest);

contactGroup.MapDelete("/phones", ([FromBody] RemovePhoneDto dto, IContactUseCases useCases) =>
    useCases.RemovePhone(dto).Match(
        _ => Results.NoContent(),
        error => Results.Problem(detail: error, statusCode: StatusCodes.Status400BadRequest)
    )
)
.WithSummary("Usuwa telefon z kontaktu")
.WithDescription("Usuwa powiązany numer telefonu z kontaktu.")
.Produces(StatusCodes.Status204NoContent)
.Produces(StatusCodes.Status400BadRequest);

contactGroup.MapPost("/important-dates", (AddImportantDateDto dto, IContactUseCases useCases) =>
    useCases.AddImportantDate(dto).Match(
        _ => Results.NoContent(),
        error => Results.Problem(detail: error, statusCode: StatusCodes.Status400BadRequest)
    )
)
.WithSummary("Dodaje ważną datę do kontaktu")
.WithDescription("Przypisuje nową ważną datę do wskazanego kontaktu.")
.Produces(StatusCodes.Status204NoContent)
.Produces(StatusCodes.Status400BadRequest);

contactGroup.MapDelete("/important-dates", ([FromBody] RemoveImportantDateDto dto, IContactUseCases useCases) =>
    useCases.RemoveImportantDate(dto).Match(
        _ => Results.NoContent(),
        error => Results.Problem(detail: error, statusCode: StatusCodes.Status400BadRequest)
    )
)
.WithSummary("Usuwa ważną datę z kontaktu")
.WithDescription("Usuwa powiązaną ważną datę z kontaktu.")
.Produces(StatusCodes.Status204NoContent)
.Produces(StatusCodes.Status400BadRequest);

var usersGroup = app.MapGroup("/api/users")
    .WithTags("Użytkownicy");

usersGroup.MapPost("", (AddUserDto dto, IUserUseCases userUseCases) =>
    userUseCases.AddUser(dto).Match(
        user => Results.Created($"/api/users/{user.Id}", user),
        error => Results.Problem(detail: error, statusCode: StatusCodes.Status400BadRequest)
    )
)
.WithSummary("Rejestruje nowego użytkownika")
.WithDescription("Tworzy nowe konto użytkownika w systemie.")
.Produces<AddUserResult>(StatusCodes.Status201Created)
.Produces(StatusCodes.Status400BadRequest);

usersGroup.MapGet("", (IUserQueries queries) =>
    Results.Ok(queries.GetUsers())
)
.WithSummary("Pobiera listę użytkowników")
.WithDescription("Zwraca listę wszystkich zarejestrowanych kont (wymaga roli administratora).")
.Produces<IEnumerable<UserDto>>(StatusCodes.Status200OK)
.Produces(StatusCodes.Status401Unauthorized)
.Produces(StatusCodes.Status403Forbidden)
.RequireAuthorization("AdminOnly");

usersGroup.MapDelete("/{id:guid}", (Guid id, IUserUseCases useCases) =>
    useCases.RemoveUser(id).Match(
        _ => Results.NoContent(),
        error => Results.NotFound(error)
    )
)
.WithSummary("Usuwa użytkownika")
.WithDescription("Usuwa konto wskazanego użytkownika (wymaga roli administratora).")
.Produces(StatusCodes.Status204NoContent)
.Produces(StatusCodes.Status401Unauthorized)
.Produces(StatusCodes.Status403Forbidden)
.Produces(StatusCodes.Status404NotFound)
.RequireAuthorization("AdminOnly");

usersGroup.MapPut("/profile", (UpdateProfileRequest request, IUserUseCases useCases, ClaimsPrincipal user) =>
{
    var userId = Guid.Parse(user.FindFirstValue(JwtRegisteredClaimNames.Sub)!);
    var dto = new UpdateProfileDto(userId, request.FirstName, request.LastName, request.BirthDate, request.Address);

    return useCases.UpdateProfile(dto).Match(
        _ => Results.NoContent(),
        error => Results.BadRequest(error)
    );
})
.WithSummary("Aktualizuje profil")
.WithDescription("Modyfikuje dane profilowe aktualnie zalogowanego użytkownika.")
.Produces(StatusCodes.Status204NoContent)
.Produces(StatusCodes.Status400BadRequest)
.Produces(StatusCodes.Status401Unauthorized)
.RequireAuthorization();

usersGroup.MapPut("/{id:guid}/role", (Guid id, UserRole role, IUserUseCases useCases) =>
    useCases.ChangeRole(id, role).Match(
        _ => Results.NoContent(),
        error => Results.NotFound(error)
    )
)
.WithSummary("Zmienia rolę użytkownika")
.WithDescription("Nadaje nową rolę wskazanemu użytkownikowi (wymaga roli administratora).")
.Produces(StatusCodes.Status204NoContent)
.Produces(StatusCodes.Status401Unauthorized)
.Produces(StatusCodes.Status403Forbidden)
.Produces(StatusCodes.Status404NotFound)
.RequireAuthorization("AdminOnly");

var authGroup = app.MapGroup("/api/auth")
    .WithTags("Autentykacja");

authGroup.MapPost("/login", (LoginUserDto dto, IUserUseCases userUseCases) =>
    userUseCases.Login(dto).Match(
        loginData => Results.Ok(loginData),
        _ => Results.Unauthorized()
    )
)
.WithSummary("Logowanie użytkownika")
.WithDescription("Weryfikuje dane logowania i zwraca Access Token oraz Refresh Token.")
.Produces<LoginUserResult>(StatusCodes.Status200OK)
.Produces(StatusCodes.Status401Unauthorized);

authGroup.MapPost("/refresh", ([FromBody] string refreshToken, IUserUseCases userUseCases) =>
    userUseCases.RefreshToken(refreshToken).Match(
        tokens => Results.Ok(tokens),
        _ => Results.Unauthorized()
    )
)
.WithSummary("Odświeża token dostępowy")
.WithDescription("Wymienia ważny Refresh Token na nową parę tokenów.")
.Produces<LoginUserResult>(StatusCodes.Status200OK)
.Produces(StatusCodes.Status401Unauthorized);

app.Run();

using Application.Domain;
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
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using Microsoft.OpenApi.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using WebAPI.Authorization;
using WebAPI.Helpers;
using WebAPI.Inputs;

var builder = WebApplication.CreateBuilder(args);

// 1. Rejestracja bazy danych SQLite
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("default"))
);

// 2. Rejestracja serwisów i repozytoriów w kontenerze DI
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

builder.Services.AddScoped<ICategoryQueries, CategoryQueries>();
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<ICategoryUseCases, CategoryServices>();
builder.Services.AddScoped<CategoryUniquenessChecker>();

builder.Services.AddScoped<IContactRepository, ContactRepository>();
builder.Services.AddScoped<IContactUseCases, ContactServices>();
builder.Services.AddScoped<UserUniquenessChecker>();

// 3. Obsługa błędów Problem Details i OpenAPI
builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<WebAPI.GlobalExceptionHandler>();

builder.Services.AddScoped<IUserQueries, UserQueries>();

builder.Services.AddScoped<ITokenGenerator, TokenGenerator>();
builder.Services.AddScoped<IPasswordHasher, WebAPI.Helpers.PasswordHasher>();
builder.Services.AddSingleton<IAuthorizationHandler, CategoryAuthorizationHandler>();


//builder.Services.AddOpenApi(options =>
//{
//    options.AddDocumentTransformer((document, context, ct) =>
//    {
//        document.Components ??= new OpenApiComponents();
//        document.Components.SecuritySchemes ??= new Dictionary<string, IOpenApiSecurityScheme>();
//        document.Components.SecuritySchemes["Bearer"] = new OpenApiSecurityScheme
//        {
//            Type = SecuritySchemeType.Http,
//            Scheme = "bearer",
//            BearerFormat = "JWT",
//            Description = "Wklej token JWT otrzymany z /api/login"
//        };

//        document.Security =
//        [
//            new OpenApiSecurityRequirement
//            {
//                [new OpenApiSecuritySchemeReference("Bearer", document)] = []
//            }
//        ];

//        return Task.CompletedTask;
//    });
//});Przez cos nie dziala ta wersja Bartczuka
builder.Services.AddOpenApi(options =>
{
    options.AddDocumentTransformer((document, context, ct) =>
    {
        document.Components ??= new OpenApiComponents();
        document.Components.SecuritySchemes ??= new Dictionary<string, OpenApiSecurityScheme>();
        document.Components.SecuritySchemes["Bearer"] = new OpenApiSecurityScheme
        {
            Type = SecuritySchemeType.Http,
            Scheme = "bearer",
            BearerFormat = "JWT",
            Description = "Wklej token JWT otrzymany z /api/login"
        };

        document.SecurityRequirements ??= new List<OpenApiSecurityRequirement>();
        document.SecurityRequirements.Add(new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                    }
                },
                Array.Empty<string>()
            }
        });

        return Task.CompletedTask;
    });
});

builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUserUseCases, UserServices>();
builder.Services.AddScoped<ITokenGenerator, TokenGenerator>();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(x =>
    {
        x.TokenValidationParameters = new TokenValidationParameters
        {
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["ApplicationSettings:JWT_KEY"]!)
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

var app = builder.Build();

app.UseExceptionHandler();
app.UseAuthentication();
app.UseAuthorization();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/openapi/v1.json", "API v1");
    });
}

// ============================================================================
// 1. MODUŁ KATEGORII (Zgrupowany za pomocą MapGroup)
// ============================================================================

var categories = app.MapGroup("/api/categories")
    .WithTags("Kategorie")
    .RequireAuthorization();

// GET /api/categories/{id:guid}
categories.MapGet("/{id:guid}", (
    Guid id,
    ICategoryQueries queries,
    ClaimsPrincipal user) =>
{
    var userId = Guid.Parse(user.FindFirstValue(JwtRegisteredClaimNames.Sub)!);

    return queries.GetCategory(id, userId).Match(
        category => Results.Ok(category),
        () => Results.NotFound()
    );
})
.RequireAuthorization()
.WithSummary("Pobiera kategorię zalogowanego użytkownika po ID")
.WithDescription("Zwraca dane kategorii, o ile należy ona do aktualnie uwierzytelnionego użytkownika.")
.Produces<CategoryDto>(StatusCodes.Status200OK)
.Produces(StatusCodes.Status404NotFound)
.Produces(StatusCodes.Status401Unauthorized);

// POST /api/categories
categories.MapPost("", (
    AddCategoryRequest input,
    ICategoryUseCases useCases,
    ClaimsPrincipal user) =>
    useCases
        .AddCategory(new AddCategoryDto(
            input.Name,
            Guid.Parse(user.FindFirstValue(JwtRegisteredClaimNames.Sub)!)))
        .Match(
            category => Results.Created($"/api/categories/{category.Id}", category),
            error => Results.Problem(detail: error, statusCode: 400)
        )
)
.RequireAuthorization()
.WithSummary("Dodaje kategorię")
.WithDescription("Tworzy nową kategorię i zwraca informacje o niej.")
.Produces<AddCategoryResult>(StatusCodes.Status201Created)
.Produces(StatusCodes.Status400BadRequest);

// PUT /api/categories/{id:guid}
categories.MapPut("/{id:guid}", async (
    Guid id,
    UpdateCategoryRequest request,
    ICategoryRepository categoryRepository,
    ICategoryUseCases useCases,
    IAuthorizationService authorizationService,
    ClaimsPrincipal user) =>
{
    return await categoryRepository.GetById(id).Match<Task<IResult>>(
        async category =>
        {
            // Weryfikacja uprawnień do tego konkretnego zasobu
            var authResult = await authorizationService.AuthorizeAsync(
                user, category, new SameUserOrAdminRequirement());

            if (!authResult.Succeeded)
            {
                return Results.Forbid(); // 403 Forbidden
            }

            var dto = new UpdateCategoryDto(id, request.Name);
            return useCases.UpdateCategory(dto).Match(
                _ => Results.NoContent(),
                error => Results.Problem(detail: error, statusCode: 400)
            );
        },
        () => Task.FromResult(Results.NotFound()) // 404 NotFound
    );
})
.RequireAuthorization()
.WithSummary("Aktualizuje kategorię (Właściciel lub Admin)")
.Produces(StatusCodes.Status204NoContent)
.Produces(StatusCodes.Status400BadRequest)
.Produces(StatusCodes.Status401Unauthorized)
.Produces(StatusCodes.Status403Forbidden)
.Produces(StatusCodes.Status404NotFound);

// DELETE /api/categories/{id:guid}
categories.MapDelete("/{id:guid}", async (
    Guid id,
    ICategoryRepository categoryRepository,
    ICategoryUseCases useCases,
    IAuthorizationService authorizationService,
    ClaimsPrincipal user) =>
{
    return await categoryRepository.GetById(id).Match<Task<IResult>>(
        async category =>
        {
            // Weryfikacja uprawnień do tego konkretnego zasobu
            var authResult = await authorizationService.AuthorizeAsync(
                user, category, new SameUserOrAdminRequirement());

            if (!authResult.Succeeded)
            {
                return Results.Forbid(); // 403 Forbidden
            }

            return useCases.RemoveCategory(id).Match(
                _ => Results.NoContent(),
                error => Results.Problem(detail: error, statusCode: 400)
            );
        },
        () => Task.FromResult(Results.NotFound()) // 404 NotFound
    );
})
.RequireAuthorization()
.WithSummary("Usuwa kategorię (Właściciel lub Admin)")
.Produces(StatusCodes.Status204NoContent)
.Produces(StatusCodes.Status400BadRequest)
.Produces(StatusCodes.Status401Unauthorized)
.Produces(StatusCodes.Status403Forbidden)
.Produces(StatusCodes.Status404NotFound);

// DELETE /api/categories/{id:guid}
categories.MapDelete("/{id:guid}", (Guid id, ICategoryUseCases useCases) =>
    useCases.RemoveCategory(id).Match(
        _ => Results.NoContent(),
        error => Results.Problem(detail: error, statusCode: 400)
    )
)
.WithSummary("Usuwa kategorię")
.WithDescription("Usuwa wybraną kategorię pod warunkiem, że nie jest przypisana do żadnego kontaktu.")
.Produces(StatusCodes.Status204NoContent)
.Produces(StatusCodes.Status400BadRequest);

// ============================================================================
// 2. MODUŁ KONTAKTÓW
// ============================================================================

// POST /api/contacts
app.MapPost("/api/contacts", (
    AddContactRequest request,
    IContactUseCases useCases,
    ClaimsPrincipal user) =>
{
    var userId = Guid.Parse(user.FindFirstValue(JwtRegisteredClaimNames.Sub)!);
    var dto = new AddContactDto(request.FirstName, request.LastName, request.Sex, request.Age, userId);

    return useCases.AddContact(dto).Match(
        contact => Results.Created($"/api/contacts/{contact.Id}", contact),
        error => Results.Problem(detail: error, statusCode: 400)
    );
})
.RequireAuthorization()
.WithTags("Kontakty")
.WithSummary("Tworzy nowy kontakt przypisany do zalogowanego użytkownika")
.Produces<AddContactResult>(StatusCodes.Status201Created)
.Produces(StatusCodes.Status400BadRequest)
.Produces(StatusCodes.Status401Unauthorized);

// PUT /api/contacts/{id:guid}
app.MapPut("/api/contacts/{id:guid}", (Guid id, UpdateContactDto dto, IContactUseCases useCases) =>
{
    var updateDto = new UpdateContactDto(id, dto.FirstName, dto.LastName, dto.Sex, dto.Age);
    return useCases.UpdateContact(updateDto).Match(
        _ => Results.NoContent(),
        error => Results.Problem(detail: error, statusCode: 400)
    );
})
.RequireAuthorization()
.WithSummary("Aktualizuje dane kontaktu")
.WithTags("Kontakty")
.Produces(StatusCodes.Status204NoContent)
.Produces(StatusCodes.Status400BadRequest)
.Produces(StatusCodes.Status401Unauthorized);

// DELETE /api/contacts/{id:guid}
app.MapDelete("/api/contacts/{id:guid}", (Guid id, IContactUseCases useCases) =>
    useCases.RemoveContact(id).Match(
        _ => Results.NoContent(),
        error => Results.Problem(detail: error, statusCode: 400)
    )
)
.RequireAuthorization()
.WithSummary("Usuwa kontakt")
.WithTags("Kontakty")
.Produces(StatusCodes.Status204NoContent)
.Produces(StatusCodes.Status400BadRequest)
.Produces(StatusCodes.Status401Unauthorized);

// ============================================================================
// 3. PODZASOBY: ADRESY E-MAIL
// ============================================================================

app.MapPost("/api/contacts/{contactId:guid}/emails", (Guid contactId, AddEmailRequest request, IContactUseCases useCases) =>
{
    var dto = new AddEmailDto(contactId, request.Address, request.CategoryId);
    return useCases.AddEmail(dto).Match(
        _ => Results.Ok(),
        error => Results.Problem(detail: error, statusCode: 400)
    );
})
.RequireAuthorization()
.WithSummary("Dodaje adres e-mail")
.WithDescription("Dołącza do kontaktu nowy adres e-mail przypisany do konkretnej kategorii.")
.WithTags("Kontakty - E-maile")
.Produces(StatusCodes.Status200OK)
.Produces(StatusCodes.Status400BadRequest)
.Produces(StatusCodes.Status401Unauthorized);

app.MapDelete("/api/contacts/{contactId:guid}/emails", (Guid contactId, string address, IContactUseCases useCases) =>
{
    var dto = new RemoveEmailDto(contactId, address);
    return useCases.RemoveEmail(dto).Match(
        _ => Results.NoContent(),
        error => Results.Problem(detail: error, statusCode: 400)
    );
})
.RequireAuthorization()
.WithSummary("Usuwa adres e-mail")
.WithDescription("Usuwa wskazany adres e-mail z podanego kontaktu.")
.WithTags("Kontakty - E-maile")
.Produces(StatusCodes.Status204NoContent)
.Produces(StatusCodes.Status400BadRequest)
.Produces(StatusCodes.Status401Unauthorized);

// ============================================================================
// 4. PODZASOBY: NUMERY TELEFONÓW
// ============================================================================

app.MapPost("/api/contacts/{contactId:guid}/phones", (Guid contactId, AddPhoneRequest request, IContactUseCases useCases) =>
{
    var dto = new AddPhoneDto(contactId, request.Number, request.CategoryId);
    return useCases.AddPhone(dto).Match(
        _ => Results.Ok(),
        error => Results.Problem(detail: error, statusCode: 400)
    );
})
.RequireAuthorization()
.WithSummary("Dodaje numer telefonu")
.WithDescription("Dołącza do kontaktu numer telefonu przypisany do określonej kategorii.")
.WithTags("Kontakty - Telefony")
.Produces(StatusCodes.Status200OK)
.Produces(StatusCodes.Status400BadRequest)
.Produces(StatusCodes.Status401Unauthorized);

app.MapDelete("/api/contacts/{contactId:guid}/phones", (Guid contactId, string number, IContactUseCases useCases) =>
{
    var dto = new RemovePhoneDto(contactId, number);
    return useCases.RemovePhone(dto).Match(
        _ => Results.NoContent(),
        error => Results.Problem(detail: error, statusCode: 400)
    );
})
.RequireAuthorization()
.WithSummary("Usuwa numer telefonu")
.WithDescription("Usuwa wskazany numer telefonu z kontaktu.")
.WithTags("Kontakty - Telefony")
.Produces(StatusCodes.Status204NoContent)
.Produces(StatusCodes.Status400BadRequest)
.Produces(StatusCodes.Status401Unauthorized);

// ============================================================================
// 5. UŻYTKOWNICY I LOGOWANIE (PUBLICZNE - BEZ WYMAGANIA TOKENU)
// ============================================================================

app.MapPost("/api/users", (AddUserDto dto, IUserUseCases userUseCases) =>
    userUseCases
        .AddUser(dto)
        .Match(
            user => Results.Created($"/api/users/{user.Id}", user),
            error => Results.Problem(detail: error, statusCode: 400)
        )
)
.WithTags("Użytkownicy")
.WithSummary("Rejestracja nowego użytkownika")
.Produces<AddUserResult>(StatusCodes.Status201Created)
.Produces(StatusCodes.Status400BadRequest);

// POST /api/login
app.MapPost("/api/login", (LoginUserDto dto, IUserUseCases userUseCases) =>
    userUseCases.Login(dto).Match(
        loginData => Results.Ok(loginData),
        _ => Results.Unauthorized()
    )
)
.WithTags("Użytkownicy")
.WithSummary("Logowanie użytkownika")
.Produces<LoginUserResult>(StatusCodes.Status200OK)
.Produces(StatusCodes.Status401Unauthorized);

// POST /api/refresh-token - PUBLICZNY
app.MapPost("/api/refresh-token", (RefreshTokenRequest request, IUserUseCases userUseCases) =>
    userUseCases.RefreshToken(new RefreshTokenDto(request.RefreshToken)).Match(
        loginData => Results.Ok(loginData),
        error => Results.Problem(detail: error, statusCode: 400)
    )
)
.WithTags("Użytkownicy")
.WithSummary("Odświeża token dostępowy za pomocą ważnego refresh tokenu")
.Produces<LoginUserResult>(StatusCodes.Status200OK)
.Produces(StatusCodes.Status400BadRequest);
// GET /api/users - TYLKO DLA ADMINA
app.MapGet("/api/users", (IUserQueries userQueries) =>
    Results.Ok(userQueries.GetAll())
)
.RequireAuthorization("AdminOnly")
.WithTags("Użytkownicy")
.WithSummary("Pobiera listę użytkowników (Tylko Admin)")
.Produces<IEnumerable<UserDto>>(StatusCodes.Status200OK)
.Produces(StatusCodes.Status401Unauthorized)
.Produces(StatusCodes.Status403Forbidden);

// PUT /api/users/profile - DLA KAŻDEGO ZALOGOWANEGO UŻYTKOWNIKA (ID Z TOKENU)
app.MapPut("/api/users/profile", (
    UpdateUserProfileRequest request,
    IUserUseCases userUseCases,
    ClaimsPrincipal user) =>
{
    var userId = Guid.Parse(user.FindFirstValue(JwtRegisteredClaimNames.Sub)!);
    var dto = new UpdateUserProfileDto(userId, request.FirstName, request.LastName, request.DateOfBirth, request.Address);

    return userUseCases.UpdateProfile(dto).Match(
        _ => Results.NoContent(),
        error => Results.Problem(detail: error, statusCode: 400)
    );
})
.RequireAuthorization()
.WithTags("Użytkownicy")
.WithSummary("Aktualizuje profil zalogowanego użytkownika")
.Produces(StatusCodes.Status204NoContent)
.Produces(StatusCodes.Status400BadRequest)
.Produces(StatusCodes.Status401Unauthorized);

// DELETE /api/users/{id:guid} - TYLKO DLA ADMINA
app.MapDelete("/api/users/{id:guid}", (Guid id, IUserUseCases userUseCases) =>
    userUseCases.RemoveUser(id).Match(
        _ => Results.NoContent(),
        error => Results.Problem(detail: error, statusCode: 400)
    )
)
.RequireAuthorization("AdminOnly")
.WithTags("Użytkownicy")
.WithSummary("Usuwa użytkownika (Tylko Admin)")
.Produces(StatusCodes.Status204NoContent)
.Produces(StatusCodes.Status400BadRequest)
.Produces(StatusCodes.Status401Unauthorized)
.Produces(StatusCodes.Status403Forbidden);

// PUT /api/users/{id:guid}/role - TYLKO DLA ADMINA
app.MapPut("/api/users/{id:guid}/role", (
    Guid id,
    ChangeRoleRequest request,
    IUserUseCases userUseCases) =>
{
    var dto = new ChangeUserRoleDto(id, request.Role);
    return userUseCases.ChangeRole(dto).Match(
        _ => Results.NoContent(),
        error => Results.Problem(detail: error, statusCode: 400)
    );
})
.RequireAuthorization("AdminOnly")
.WithTags("Użytkownicy")
.WithSummary("Zmienia rolę użytkownika (Tylko Admin)")
.Produces(StatusCodes.Status204NoContent)
.Produces(StatusCodes.Status400BadRequest)
.Produces(StatusCodes.Status401Unauthorized)
.Produces(StatusCodes.Status403Forbidden);

// ============================================================================
// 6. PODZASOBY: WAŻNE DATY
// ============================================================================

app.MapPost("/api/contacts/{contactId:guid}/dates", (Guid contactId, AddImportantDateRequest request, IContactUseCases useCases) =>
{
    var dto = new AddImportantDateDto(contactId, request.Date, request.Description);
    return useCases.AddImportantDate(dto).Match(
        _ => Results.Ok(),
        error => Results.Problem(detail: error, statusCode: 400)
    );
})
.RequireAuthorization()
.WithSummary("Dodaje ważną datę")
.WithDescription("Zapisuje powiązaną z kontaktem ważną datę wraz z jej opisem.")
.WithTags("Kontakty - Ważne daty")
.Produces(StatusCodes.Status200OK)
.Produces(StatusCodes.Status400BadRequest)
.Produces(StatusCodes.Status401Unauthorized);

app.MapDelete("/api/contacts/{contactId:guid}/dates", (Guid contactId, string description, IContactUseCases useCases) =>
{
    var dto = new RemoveImportantDateDto(contactId, description);
    return useCases.RemoveImportantDate(dto).Match(
        _ => Results.NoContent(),
        error => Results.Problem(detail: error, statusCode: 400)
    );
})
.RequireAuthorization()
.WithSummary("Usuwa ważną datę")
.WithDescription("Usuwa wpis o dacie z kontaktu na podstawie jej opisu.")
.WithTags("Kontakty - Ważne daty")
.Produces(StatusCodes.Status204NoContent)
.Produces(StatusCodes.Status400BadRequest)
.Produces(StatusCodes.Status401Unauthorized);

// Automatyczne utworzenie konta administratora przy starcie (jeśli brak w bazie)
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    var hasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher>();

    if (!dbContext.Users.Any(u => u.Role == UserRole.Admin))
    {
        User.Create("admin", "Admin123!")
            .Tap(admin =>
            {
                admin.ChangeRole(UserRole.Admin);
                admin.SetPassword(hasher.HashPassword("Admin123!"));
                admin.UpdateProfile("Główny", "Administrator", null, null);
                dbContext.Users.Add(admin);
                dbContext.SaveChanges();
            });
    }
}

app.Run();

// Obiekty pomocnicze do przesyłania ciał żądań podzasobów
public record AddEmailRequest(string Address, Guid CategoryId);
public record AddPhoneRequest(string Number, Guid CategoryId);
public record AddImportantDateRequest(DateOnly Date, string Description);
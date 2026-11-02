using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.IdentityModel.Tokens.Jwt;
using WebAPI.Inputs;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Application.DomainServices;
using Application.OutputPorts; 
using WebAPI.Helpers;
using Application.Queries;
using Application.Queries.Dtos;
using Application.Repositories; 
using Application.Services; 
using Application.UseCases;
using Application.UseCases.Dtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Authorization;
using WebAPI.Authorization;
using Data;

var builder = WebApplication.CreateBuilder(args);
 builder.Services.AddDbContext<AppDbContext>(options =>
     options.UseSqlite(builder.Configuration.GetConnectionString("default"))
);
// Add services to the container.
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
builder.Services.AddScoped<ITokenGenerator, TokenGenerator>();
builder.Services.AddScoped<IPasswordHasher, PasswordHasher>();
builder.Services.AddScoped<ICategoryQueries, CategoryQueries>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<ICategoryUseCases, CategoryServices>();
builder.Services.AddScoped<CategoryUniquenessChecker>();
builder.Services.AddScoped<UserUniquenessChecker>();
builder.Services.AddScoped<IContactRepository, ContactRepository>();
builder.Services.AddScoped<IContactUseCases, ContactServices>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUserUseCases, UserServices>();
builder.Services.AddSingleton<IAuthorizationHandler, CategoryOwnerAuthorization>();
builder.Services.AddProblemDetails(); 
builder.Services.AddExceptionHandler<WebAPI.GlobalExceptionHandler>();
builder.Services.AddAuthorizationBuilder()
     .AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi(options => {
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

var app = builder.Build();
app.UseExceptionHandler();
// Configure the HTTP request pipeline.

    app.MapOpenApi();
    app.UseSwaggerUI(options =>
     {
         options.SwaggerEndpoint("/openapi/v1.json", "API v1");
         options.RoutePrefix = "swagger";
     });


app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapPost("/api/login", (LoginUserDto dto, IUserUseCases userUseCases) =>
     userUseCases.Login(dto).Match(
         loginData => Results.Ok(loginData),
         _ => Results.Unauthorized()
     )
)
.WithSummary("Logowanie użytkownika")
.WithDescription("Zwraca AccessToken (ważny 15 minut) oraz RefreshToken (ważny 7 dni).")
.Produces<TokenResponseDto>(StatusCodes.Status200OK)
.Produces(StatusCodes.Status401Unauthorized);

app.MapPost("/api/refresh-token", (RefreshTokenRequest request, IUserUseCases userUseCases) =>
    userUseCases.RefreshToken(new RefreshTokenDto(request.RefreshToken)).Match(
        tokens => Results.Ok(tokens),
        error => Results.BadRequest(error)
    )
)
.WithSummary("Odświeża token dostępowy za pomocą refresh tokena")
.Produces<TokenResponseDto>(StatusCodes.Status200OK)
.Produces(StatusCodes.Status400BadRequest);

var usersGroup = app.MapGroup("/api/users")
    .WithTags("Użytkownicy");

usersGroup.MapPost("/", (AddUserDto dto, IUserUseCases userUseCases) =>
    userUseCases
        .AddUser(dto)
        .Match(
            user => Results.Created($"/api/users/{user.Id}", user),
            error => Results.Problem(detail: error, statusCode: 400)
        )
)
.WithSummary("Rejestruje nowego użytkownika")
.Produces<AddUserResult>(StatusCodes.Status201Created)
.Produces(StatusCodes.Status400BadRequest);

usersGroup.MapGet("/", (IUserUseCases useCases) =>
    useCases.GetUsers().Match(
        users => Results.Ok(users),
        error => Results.BadRequest(error)
    )
)
.RequireAuthorization("AdminOnly")
.WithSummary("Pobiera listę wszystkich użytkowników")
.Produces<IEnumerable<UserDto>>(StatusCodes.Status200OK);

usersGroup.MapPut("/", (UpdateUserDto dto, IUserUseCases useCases, ClaimsPrincipal user) =>
{
    var userIdString = user.FindFirstValue(JwtRegisteredClaimNames.Sub);

    if (userIdString is null || !Guid.TryParse(userIdString, out var userId))
    {
        return Results.Unauthorized();
    }

    return useCases.UpdateUser(userId, dto).Match(
        _ => Results.NoContent(),
        error => Results.Problem(detail: error, statusCode: 400)
    );
})
.RequireAuthorization()
.WithSummary("Aktualizuje profil zalogowanego użytkownika")
.Produces(StatusCodes.Status204NoContent)
.Produces(StatusCodes.Status400BadRequest);

usersGroup.MapDelete("/{id:guid}", (Guid id, IUserUseCases useCases, IAuthorizationService authService, 
    ClaimsPrincipal user) =>
    useCases.RemoveUser(id).Match(
        _ => Results.NoContent(),
        error => Results.Problem(detail: error, statusCode: 400)
    )
)
.RequireAuthorization("AdminOnly")
.WithSummary("Usuwa użytkownika o podanym ID")
.Produces(StatusCodes.Status204NoContent)
.Produces(StatusCodes.Status400BadRequest);

usersGroup.MapPatch("/{id:guid}/role", (Guid id, ChangeUserRoleDto dto, IUserUseCases useCases) =>
    useCases.ChangeRole(id, dto).Match(
        _ => Results.NoContent(),
        error => Results.Problem(detail: error, statusCode: 400)
    )
)
.RequireAuthorization("AdminOnly")
.WithSummary("Zmienia rolę użytkownika")
.WithDescription("Zmienia rolę wskazanego użytkownika (0 = User, 1 = Admin). Wymaga roli Admin.")
.Produces(StatusCodes.Status204NoContent)
.Produces(StatusCodes.Status400BadRequest);

var categoriesGroup = app.MapGroup("/api/categories")
    .WithTags("Kategorie")
    .RequireAuthorization();

 categoriesGroup.MapPost("/",
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
.WithSummary("Dodaje kategorię")
.WithDescription("Tworzy nową kategorię i zwraca utworzony obiekt.")
.Produces<AddCategoryResult>(StatusCodes.Status201Created) 
.Produces(StatusCodes.Status400BadRequest);

categoriesGroup.MapGet("/",
    (ICategoryQueries queries, ClaimsPrincipal user) => Results.Ok(queries.GetCategories(Guid.Parse(user.FindFirstValue(JwtRegisteredClaimNames.Sub)!)))
)
.RequireAuthorization()
.WithSummary("Pobiera kategorię")
.WithDescription("Zwraca listę kategorii")
.WithTags("Kategorie")
.Produces<IEnumerable<CategoryDto>>(StatusCodes.Status200OK) 
.Produces(StatusCodes.Status400BadRequest);

categoriesGroup.MapDelete("/{id:Guid}", async (
    Guid id, 
    ICategoryUseCases useCases, 
    IAuthorizationService authService, 
    ClaimsPrincipal user) => 
{
    var authResult = await authService.AuthorizeAsync(user, id, new CategoryOwnerRequirement());
    if (!authResult.Succeeded)
    {
        return Results.Forbid();
    }

    return useCases.RemoveCategory(id).Match(
        _ => Results.NoContent(),
        error => Results.Problem(detail: error, statusCode: 400)
    );
})
.WithSummary("Usuwa kategorię")
.WithDescription("Usuwa wskazaną kategorię na podstawie przekazanego indeksa.")
.WithTags("Kategorie")
.Produces(StatusCodes.Status204NoContent) 
.Produces(StatusCodes.Status400BadRequest);

categoriesGroup.MapGet("/{id:Guid}", (Guid id, ICategoryQueries queries, ClaimsPrincipal user) =>
{
    var userIdString = user.FindFirstValue(JwtRegisteredClaimNames.Sub);
    
    if (userIdString is null || !Guid.TryParse(userIdString, out var userId))
    {
        return Results.Unauthorized();
    }

    return queries.GetCategory(id, userId) is { } category
        ? Results.Ok(category)
        : Results.NotFound();
}
)
.WithSummary("Pobiera kategorię po id")
.WithDescription("Zwraca szczegóły kategorii na podstawie jej indeksa.")
.WithTags("Kategorie")
.Produces<CategoryDto>(StatusCodes.Status200OK)
.Produces(StatusCodes.Status404NotFound);

categoriesGroup.MapPut("/", async (
    UpdateCategoryDto dto, 
    ICategoryUseCases useCases, 
    IAuthorizationService authService, 
    ClaimsPrincipal user) => 
{
    var authResult = await authService.AuthorizeAsync(user, dto.Id, new CategoryOwnerRequirement());
    if (!authResult.Succeeded)
    {
        return Results.Forbid();
    }

    return useCases
        .UpdateCategory(dto)
        .Match(
            _ => Results.NoContent(),
            error => Results.Problem(detail: error, statusCode: 400)
        );
})
.WithSummary("Aktualizuje kategorię")
.WithDescription("Aktualizuje kategorię na podstawie przesłanych danych.")
.WithTags("Kategorie")
.Produces(StatusCodes.Status204NoContent) 
.Produces(StatusCodes.Status400BadRequest);

var contactsGroup = app.MapGroup("/api/contacts")
    .WithTags("Kontakty")
    .RequireAuthorization();


contactsGroup.MapPost("/",
    (AddContactRequest input,
     IContactUseCases useCases,
     ClaimsPrincipal user) =>
        useCases
            .AddContact(new AddContactDto(
                Guid.Parse(user.FindFirstValue(JwtRegisteredClaimNames.Sub)!),
                input.FirstName,
                input.LastName,
                input.Sex,
                input.Age)
            ) 
            .Match(
                contact =>
                    Results.Created(
                        $"/api/contacts/{contact.Id}",
                        contact),
                error => Results.BadRequest(error)
            )
)
.WithSummary("Dodaje kontakt")
.WithDescription("Tworzy nowy kontakt w systemie.")
.Produces<AddContactResult>(StatusCodes.Status201Created)
.Produces(StatusCodes.Status400BadRequest);

contactsGroup.MapPut("/", (UpdateContactDto dto, IContactUseCases useCases) =>
    useCases.UpdateContact(dto).Match(
        _ => Results.NoContent(),
        error => Results.Problem(detail: error, statusCode: 400)
    )
)
.WithSummary("Aktualizuje kontakt")
.WithDescription("Aktualizuje dane istniejącego kontaktu.")
.Produces(StatusCodes.Status204NoContent)
.Produces(StatusCodes.Status400BadRequest);

contactsGroup.MapDelete("/{id:Guid}", (Guid id, IContactUseCases useCases) =>
    useCases.RemoveContact(id).Match(
        _ => Results.NoContent(),
        error => Results.Problem(detail: error, statusCode: 400)
    )
)
.WithSummary("Usuwa kontakt")
.WithDescription("Usuwa kontakt na podstawie jego indeksu.")
.Produces(StatusCodes.Status204NoContent)
.Produces(StatusCodes.Status400BadRequest);

contactsGroup.MapPost("/emails", (AddEmailDto dto, IContactUseCases useCases) =>
    useCases.AddEmail(dto).Match(
        _ => Results.NoContent(),
        error => Results.Problem(detail: error, statusCode: 400)
    )
)
.WithSummary("Dodaje e-mail do kontaktu")
.WithTags("Kontakty - Powiązania")
.Produces(StatusCodes.Status204NoContent)
.Produces(StatusCodes.Status400BadRequest);

contactsGroup.MapDelete("/emails", ([FromBody]RemoveEmailDto dto, IContactUseCases useCases) =>
    useCases.RemoveEmail(dto).Match(
        _ => Results.NoContent(),
        error => Results.Problem(detail: error, statusCode: 400)
    )
)
.WithSummary("Usuwa e-mail z kontaktu")
.WithTags("Kontakty - Powiązania")
.Produces(StatusCodes.Status204NoContent)
.Produces(StatusCodes.Status400BadRequest);

contactsGroup.MapPost("/phones", (AddPhoneDto dto, IContactUseCases useCases) =>
    useCases.AddPhone(dto).Match(
        _ => Results.NoContent(),
        error => Results.Problem(detail: error, statusCode: 400)
    )
)
.WithSummary("Dodaje numer telefonu do kontaktu")
.WithTags("Kontakty - Powiązania")
.Produces(StatusCodes.Status204NoContent)
.Produces(StatusCodes.Status400BadRequest);

contactsGroup.MapDelete("/phones", ([FromBody]RemovePhoneDto dto, IContactUseCases useCases) =>
    useCases.RemovePhone(dto).Match(
        _ => Results.NoContent(),
        error => Results.Problem(detail: error, statusCode: 400)
    )
)
.WithSummary("Usuwa numer telefonu z kontaktu")
.WithTags("Kontakty - Powiązania")
.Produces(StatusCodes.Status204NoContent)
.Produces(StatusCodes.Status400BadRequest);

contactsGroup.MapPost("/important-dates", (AddImportantDateDto dto, IContactUseCases useCases) =>
    useCases.AddImportantDate(dto).Match(
        _ => Results.NoContent(),
        error => Results.Problem(detail: error, statusCode: 400)
    )
)
.WithSummary("Dodaje ważną datę do kontaktu")
.WithTags("Kontakty - Powiązania")
.Produces(StatusCodes.Status204NoContent)
.Produces(StatusCodes.Status400BadRequest);

contactsGroup.MapDelete("/important-dates", ([FromBody]RemoveImportantDateDto dto, IContactUseCases useCases) =>
    useCases.RemoveImportantDate(dto).Match(
        _ => Results.NoContent(),
        error => Results.Problem(detail: error, statusCode: 400)
    )
)
.WithSummary("Usuwa ważną datę z kontaktu")
.WithTags("Kontakty - Powiązania")
.Produces(StatusCodes.Status204NoContent)
.Produces(StatusCodes.Status400BadRequest);

contactsGroup.MapGet("/", (IContactRepository repository) =>
    Results.Ok(repository.GetAll()) 
)
.WithSummary("Pobiera wszystkie kontakty")
.WithDescription("Zwraca listę wszystkich kontaktów zapisanych w systemie.")
.Produces(StatusCodes.Status200OK); 

contactsGroup.MapGet("/{id:Guid}", (Guid id, IContactRepository repository) =>
    repository.GetById(id) is { } contact
        ? Results.Ok(contact)
        : Results.NotFound()
)
.WithSummary("Pobiera kontakt po Id")
.WithDescription("Zwraca szczegóły konkretnego kontaktu na podstawie jego indeksu.")
.Produces(StatusCodes.Status200OK)
.Produces(StatusCodes.Status404NotFound);
app.Run();


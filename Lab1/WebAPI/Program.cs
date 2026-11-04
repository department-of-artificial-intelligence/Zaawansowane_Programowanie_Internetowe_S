using Application.DomainServices;
using Application.Domain;
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
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using WebAPI.Helpers;
using WebAPI.Inputs;
using Microsoft.AspNetCore.Identity;

var builder = WebApplication.CreateBuilder(args);

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
builder.Services.AddDbContext<AppDbContext>(options =>

    options.UseSqlite(builder.Configuration.GetConnectionString("default")));
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["ApplicationSettings:JWT_KEY"]!)),
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            NameClaimType = "name",
            RoleClaimType = "role"
        };
        options.MapInboundClaims = false;
    });
builder.Services.AddAuthorizationBuilder()
    .AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"))
    .AddPolicy("CategoryOwner", policy => policy.AddRequirements(new CategoryOwnerRequirement()));
builder.Services.AddScoped<ICategoryQueries, CategoryQueries>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<IContactRepository, ContactRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<ICategoryUseCases, CategoryServices>();
builder.Services.AddScoped<IContactUseCases, ContactServices>();
builder.Services.AddScoped<IUserUseCases, UserServices>();
builder.Services.AddScoped<ITokenGenerator, TokenGenerator>();
builder.Services.AddScoped<IPasswordHasher, PasswordHasherAdapter>();
builder.Services.AddScoped<UserUniquenessChecker>();
builder.Services.AddSingleton<IAuthorizationHandler, CategoryOwnerHandler>();
builder.Services.AddScoped<CategoryUniquenessChecker>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    dbContext.Database.Migrate();
}

if (app.Environment.IsDevelopment())
{
    app.UseSwaggerUI(options =>
        options.SwaggerEndpoint("/openapi/v1.json", "API v1"));
}

app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapPost("/api/users", (AddUserDto dto, IUserUseCases userUseCases) =>
    userUseCases.AddUser(dto).Match(
        user => Results.Created($"/api/users/{user.Id}", user),
        error => Results.Problem(detail: error, statusCode: StatusCodes.Status400BadRequest)));

app.MapPost("/api/login", (LoginUserDto dto, IUserUseCases userUseCases) =>
userUseCases.Login(dto).Match(
loginData => Results.Ok(loginData.AccessToken),
_ => Results.Unauthorized()
)
);

app.MapPost("/api/refresh", (RefreshTokenDto dto, IUserUseCases userUseCases) =>
    userUseCases.Refresh(dto).Match(
        result => Results.Ok(result),
        _ => Results.Unauthorized()));

app.MapGet("/api/users", (IUserUseCases users) => Results.Ok(users.GetAll()))
    .RequireAuthorization("AdminOnly");

app.MapPut("/api/users/{id:guid}/profile", (Guid id, UpdateProfileDto dto, ClaimsPrincipal user, IUserUseCases users) =>
    id == Guid.Parse(user.FindFirstValue(JwtRegisteredClaimNames.Sub)!)
        ? users.UpdateProfile(id, dto).Match(_ => Results.NoContent(), error => Results.BadRequest(error))
        : Results.Forbid())
    .RequireAuthorization();

app.MapPut("/api/users/{id:guid}/role", (Guid id, UserRole role, IUserUseCases users) =>
    users.ChangeRole(id, role).Match(_ => Results.NoContent(), error => Results.NotFound(error)))
    .RequireAuthorization("AdminOnly");

app.MapDelete("/api/users/{id:guid}", (Guid id, IUserUseCases users) =>
    users.Remove(id).Match(_ => Results.NoContent(), error => Results.NotFound(error)))
    .RequireAuthorization("AdminOnly");


var categories = app.MapGroup("/api/categories")
    .WithTags("Kategorie");

categories.MapGet("/", (ICategoryQueries queries, ClaimsPrincipal user) =>
    Guid.TryParse(user.FindFirstValue(JwtRegisteredClaimNames.Sub), out var userId)
        ? Results.Ok(queries.GetCategories(userId))
        : Results.Unauthorized())
    .RequireAuthorization()
    .WithName("GetCategories")
    .WithSummary("Pobiera wszystkie kategorie")
    .WithDescription("Zwraca listę wszystkich kategorii.")
    .Produces<IEnumerable<CategoryDto>>(StatusCodes.Status200OK)
    .Produces(StatusCodes.Status500InternalServerError);
    

categories.MapGet("/{id:guid}", (Guid id, ICategoryQueries queries, ClaimsPrincipal user) =>
    Guid.TryParse(user.FindFirstValue(JwtRegisteredClaimNames.Sub), out var userId)
        ? queries.GetCategory(id, userId).Match(
        category => Results.Ok(category),
        () => Results.NotFound())
        : Results.Unauthorized())
    .RequireAuthorization()
    .WithName("GetCategory")
    .WithSummary("Pobiera kategorię")
    .WithDescription("Zwraca kategorię o podanym identyfikatorze.")
    .Produces<CategoryDto>(StatusCodes.Status200OK)
    .Produces(StatusCodes.Status404NotFound)
    .Produces(StatusCodes.Status500InternalServerError);

categories.MapPost("/", (AddCategoryRequest input, ClaimsPrincipal user, ICategoryUseCases useCases) =>
    Guid.TryParse(user.FindFirstValue(JwtRegisteredClaimNames.Sub), out var userId)
        ? useCases.AddCategory(new AddCategoryDto(input.Name, userId)).Match(
        category => Results.Created($"/api/categories/{category.Id}", category),
        error => Results.Problem(detail: error, statusCode: StatusCodes.Status400BadRequest))
        : Results.Unauthorized())
    .RequireAuthorization()
    .WithName("CreateCategory")
    .WithSummary("Dodaje kategorię")
    .WithDescription("Tworzy nową kategorię.")
    .Produces<AddCategoryResult>(StatusCodes.Status201Created)
    .Produces(StatusCodes.Status400BadRequest)
    .Produces(StatusCodes.Status500InternalServerError);

categories.MapPut("/{id:guid}", async (
    Guid id,
    RenameCategoryDto dto,
    ClaimsPrincipal user,
    ICategoryRepository repository,
    IAuthorizationService authorization,
    ICategoryUseCases useCases) =>
{
    var category = repository.GetById(id);
    return await category.Match(
        async value =>
        {
            var result = await authorization.AuthorizeAsync(user, value, "CategoryOwner");
            return result.Succeeded
                ? useCases.RenameCategory(new RenameCategoryDto(id, dto.Name)).Match(
                    _ => Results.NoContent(),
                    error => Results.Problem(detail: error, statusCode: StatusCodes.Status400BadRequest))
                : Results.Forbid();
        },
        () => Task.FromResult<IResult>(Results.NotFound()));
})
    .RequireAuthorization()
    .WithName("RenameCategory")
    .WithSummary("Zmienia nazwę kategorii")
    .WithDescription("Zmienia nazwę istniejącej kategorii.")
    .Produces(StatusCodes.Status204NoContent)
    .Produces(StatusCodes.Status400BadRequest)
    .Produces(StatusCodes.Status500InternalServerError);

categories.MapDelete("/{id:guid}", async (
    Guid id,
    ClaimsPrincipal user,
    ICategoryRepository repository,
    IAuthorizationService authorization,
    ICategoryUseCases useCases) =>
{
    var category = repository.GetById(id);
    return await category.Match(
        async value =>
        {
            var result = await authorization.AuthorizeAsync(user, value, "CategoryOwner");
            return result.Succeeded
                ? useCases.RemoveCategory(id).Match(
                    _ => Results.NoContent(),
                    error => Results.Problem(detail: error, statusCode: StatusCodes.Status400BadRequest))
                : Results.Forbid();
        },
        () => Task.FromResult<IResult>(Results.NotFound()));
})
    .RequireAuthorization()
    .WithName("DeleteCategory")
    .WithSummary("Usuwa kategorię")
    .WithDescription("Usuwa kategorię, jeśli nie jest przypisana do żadnego adresu e-mail.")
    .Produces(StatusCodes.Status204NoContent)
    .Produces(StatusCodes.Status400BadRequest)
    .Produces(StatusCodes.Status500InternalServerError);

var contacts = app.MapGroup("/api/contacts")
    .WithTags("Kontakty")
    .RequireAuthorization();

contacts.MapPost("/", (AddContactRequest input, ClaimsPrincipal user, IContactUseCases useCases) =>
    Guid.TryParse(user.FindFirstValue(JwtRegisteredClaimNames.Sub), out var userId)
        ? useCases.AddContact(new AddContactDto(input.FirstName, input.LastName, input.Sex, input.Age, userId)).Match(
        contact => Results.Created($"/api/contacts/{contact.Id}", contact),
        error => Results.Problem(detail: error, statusCode: StatusCodes.Status400BadRequest))
        : Results.Unauthorized())
    .WithName("CreateContact")
    .WithSummary("Dodaje kontakt")
    .WithDescription("Tworzy nowy kontakt.")
    .Produces<AddContactResult>(StatusCodes.Status201Created)
    .Produces(StatusCodes.Status400BadRequest)
    .Produces(StatusCodes.Status500InternalServerError);

contacts.MapPut("/{id:guid}", (Guid id, EditContactDto dto, IContactUseCases useCases) =>
    useCases.EditContact(new EditContactDto(id, dto.FirstName, dto.LastName, dto.Sex, dto.Age)).Match(
        _ => Results.NoContent(),
        error => Results.Problem(detail: error, statusCode: StatusCodes.Status400BadRequest)))
    .WithName("EditContact")
    .WithSummary("Edytuje kontakt")
    .WithDescription("Aktualizuje dane istniejącego kontaktu.")
    .Produces(StatusCodes.Status204NoContent)
    .Produces(StatusCodes.Status400BadRequest)
    .Produces(StatusCodes.Status500InternalServerError);

contacts.MapDelete("/{id:guid}", (Guid id, IContactUseCases useCases) =>
    useCases.RemoveContact(id).Match(
        _ => Results.NoContent(),
        error => Results.Problem(detail: error, statusCode: StatusCodes.Status400BadRequest)))
    .WithName("DeleteContact")
    .WithSummary("Usuwa kontakt")
    .WithDescription("Usuwa kontakt o podanym identyfikatorze.")
    .Produces(StatusCodes.Status204NoContent)
    .Produces(StatusCodes.Status400BadRequest)
    .Produces(StatusCodes.Status500InternalServerError);

contacts.MapPost("/{id:guid}/emails", (Guid id, AddEmailDto dto, IContactUseCases useCases) =>
    useCases.AddEmail(new AddEmailDto(id, dto.Address, dto.CategoryId)).Match(
        _ => Results.NoContent(),
        error => Results.Problem(detail: error, statusCode: StatusCodes.Status400BadRequest)))
    .WithName("AddEmail")
    .WithSummary("Dodaje adres e-mail")
    .WithDescription("Dodaje adres e-mail do kontaktu i przypisuje go do kategorii.")
    .Produces(StatusCodes.Status204NoContent)
    .Produces(StatusCodes.Status400BadRequest)
    .Produces(StatusCodes.Status500InternalServerError);

contacts.MapDelete("/{id:guid}/emails", (Guid id, [FromBody] RemoveEmailDto dto, IContactUseCases useCases) =>
    useCases.RemoveEmail(new RemoveEmailDto(id, dto.Address)).Match(
        _ => Results.NoContent(),
        error => Results.Problem(detail: error, statusCode: StatusCodes.Status400BadRequest)))
    .WithName("RemoveEmail")
    .WithSummary("Usuwa adres e-mail")
    .WithDescription("Usuwa adres e-mail z kontaktu.")
    .Produces(StatusCodes.Status204NoContent)
    .Produces(StatusCodes.Status400BadRequest)
    .Produces(StatusCodes.Status500InternalServerError);

contacts.MapPost("/{id:guid}/phone-numbers", (Guid id, PhoneNumberDto dto, IContactUseCases useCases) =>
    useCases.AddPhoneNumber(new PhoneNumberDto(id, dto.Number)).Match(
        _ => Results.NoContent(),
        error => Results.Problem(detail: error, statusCode: StatusCodes.Status400BadRequest)))
    .WithName("AddPhoneNumber")
    .WithSummary("Dodaje numer telefonu")
    .WithDescription("Dodaje numer telefonu do kontaktu.")
    .Produces(StatusCodes.Status204NoContent)
    .Produces(StatusCodes.Status400BadRequest)
    .Produces(StatusCodes.Status500InternalServerError);

contacts.MapDelete("/{id:guid}/phone-numbers", (Guid id, [FromBody] PhoneNumberDto dto, IContactUseCases useCases) =>
    useCases.RemovePhoneNumber(new PhoneNumberDto(id, dto.Number)).Match(
        _ => Results.NoContent(),
        error => Results.Problem(detail: error, statusCode: StatusCodes.Status400BadRequest)))
    .WithName("RemovePhoneNumber")
    .WithSummary("Usuwa numer telefonu")
    .WithDescription("Usuwa numer telefonu z kontaktu.")
    .Produces(StatusCodes.Status204NoContent)
    .Produces(StatusCodes.Status400BadRequest)
    .Produces(StatusCodes.Status500InternalServerError);

contacts.MapPost("/{id:guid}/important-dates", (Guid id, ImportantDateDto dto, IContactUseCases useCases) =>
    useCases.AddImportantDate(new ImportantDateDto(id, dto.Name, dto.Date)).Match(
        _ => Results.NoContent(),
        error => Results.Problem(detail: error, statusCode: StatusCodes.Status400BadRequest)))
    .WithName("AddImportantDate")
    .WithSummary("Dodaje ważną datę")
    .WithDescription("Dodaje ważną datę do kontaktu.")
    .Produces(StatusCodes.Status204NoContent)
    .Produces(StatusCodes.Status400BadRequest)
    .Produces(StatusCodes.Status500InternalServerError);

contacts.MapDelete("/{id:guid}/important-dates", (Guid id, [FromBody] ImportantDateDto dto, IContactUseCases useCases) =>
    useCases.RemoveImportantDate(new ImportantDateDto(id, dto.Name, dto.Date)).Match(
        _ => Results.NoContent(),
        error => Results.Problem(detail: error, statusCode: StatusCodes.Status400BadRequest)))
    .WithName("RemoveImportantDate")
    .WithSummary("Usuwa ważną datę")
    .WithDescription("Usuwa ważną datę z kontaktu.")
    .Produces(StatusCodes.Status204NoContent)
    .Produces(StatusCodes.Status400BadRequest)
    .Produces(StatusCodes.Status500InternalServerError);

app.Run();

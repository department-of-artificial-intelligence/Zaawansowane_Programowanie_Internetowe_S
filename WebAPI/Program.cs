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
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using WebAPI.Helpers;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using WebAPI.Inputs;
using Microsoft.AspNetCore.Authorization;

namespace WebAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // Przygotowanie konfiguratora naszej aplikacji
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddDbContext<AppDbContext>(options =>
                options.UseSqlite(builder.Configuration.GetConnectionString("default"))
            );

            builder.Services.AddScoped<ICategoryQueries, CategoryQueries>();
            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
            builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
            builder.Services.AddScoped<ICategoryUseCases, CategoryServices>();
            builder.Services.AddScoped<IContactRepository, ContactRepository>();
            builder.Services.AddScoped<IContactUseCases, ContactServices>();

            builder.Services.AddScoped<IUserRepository, UserRepository>();
            builder.Services.AddScoped<IUserUseCases, UserServices>();

            builder.Services.AddScoped<CategoryUniquenessChecker>();
            builder.Services.AddScoped<UserUniquenessChecker>();

            builder.Services.AddScoped<IAuthorizationHandler, CategoryAuthorizationHandler>();

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

            builder.Services.AddAuthorizationBuilder()
                .AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));


            builder.Services.AddScoped<ITokenGenerator, TokenGenerator>();
            builder.Services.AddScoped<IPasswordHasher, PasswordHasherService>();


            // Dodanie usług koniecznych dla OpenAPI z konfiguracją JWT Bearer w Swaggerze
            builder.Services.AddOpenApi(options =>
            {
                options.AddDocumentTransformer((document, context, ct) =>
                {
                    document.Components ??= new OpenApiComponents();
                    document.Components.SecuritySchemes ??= new Dictionary<string, OpenApiSecurityScheme>();

                    // 1. Definicja schematu uwierzytelniania
                    var bearerScheme = new OpenApiSecurityScheme
                    {
                        Type = SecuritySchemeType.Http,
                        Scheme = "bearer",
                        BearerFormat = "JWT",
                        Description = "Wklej token JWT otrzymany z /api/login"
                    };

                    document.Components.SecuritySchemes["Bearer"] = bearerScheme;

                    // 2. Referencja do schematu dla globalnych wymagań bezpieczeństwa
                    var securityRequirementScheme = new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    };

                    document.SecurityRequirements ??= new List<OpenApiSecurityRequirement>();
                    document.SecurityRequirements.Add(new OpenApiSecurityRequirement
                    {
                        [securityRequirementScheme] = Array.Empty<string>()
                    });

                    return Task.CompletedTask;
                });
            });

            // Zbudowanie aplikacji na podstawie konfiguracji
            var app = builder.Build();

            app.UseExceptionHandler();

            // Mapowanie endpoint OpenApi (tylko w przypadku trybu deweloperskiego).
            if (app.Environment.IsDevelopment())
            {
                app.UseSwaggerUI(options =>
                {
                    app.MapOpenApi();
                    options.SwaggerEndpoint("/openapi/v1.json", "API v1");
                });
            }
            // Uruchomienie przekierowania żądań z HTTP do HTTPS
            app.UseHttpsRedirection();

            //ENDPOINTY Kategorii
            var categoriesGroup = app.MapGroup("/api/categories")
                .WithTags("Kategorie")
                .RequireAuthorization();

            categoriesGroup.MapGet("",
                (ICategoryQueries queries, ClaimsPrincipal user) => 
                    Results.Ok(queries.GetCategories(
                        Guid.Parse(user.FindFirstValue(JwtRegisteredClaimNames.Sub)!)))
            )
            .RequireAuthorization()
            .WithSummary("Pobiera wszystkie kategorie")
            .WithDescription("Zwraca listę wszystkich dostępnych w systemie kategorii.")
            .Produces<IEnumerable<CategoryDto>>(StatusCodes.Status200OK);

            categoriesGroup.MapGet("{id:guid}",
                (Guid id, ICategoryQueries queries) =>
                queries.GetCategory(id).Match(
                    category => Results.Ok(category),
                    () => Results.NotFound()
                )
            )
            .WithSummary("Pobiera kategorię po ID")
            .WithDescription("Wyszukuje i zwraca szczegóły pojedynczej kategorii na podstawie jej unikalnego identyfikatora.")
            .Produces<CategoryDto>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound);

            categoriesGroup.MapPost("",
                (AddCategoryRequest input, ICategoryUseCases useCases, ClaimsPrincipal user) =>
                    useCases.AddCategory(new AddCategoryDto(
                        input.Name,
                        Guid.Parse(user.FindFirstValue(JwtRegisteredClaimNames.Sub)!)
                    ))
                    .Match(
                        category => Results.Created(
                            $"/api/categories/{category.Id}",
                            category),
                        error => Results.Problem(detail: error, statusCode: 400)
                )
            )
            .RequireAuthorization()
            .WithSummary("Dodaje kategorię")
            .WithDescription("Tworzy nową kategorię dla zalogowanego użytkownika i zwraca utworzony obiekt.")
            .Produces<AddCategoryResult>(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status400BadRequest);

            // Modyfikacja z weryfikacją właściciela zasobu
            categoriesGroup.MapPut("{id:guid}", async (
                Guid id,
                UpdateCategoryDto dto,
                ICategoryQueries queries,
                ICategoryUseCases useCases,
                IAuthorizationService authService,
                ClaimsPrincipal user) =>
            {
                var categoryOption = queries.GetCategory(id);

                return await categoryOption.Match(
                    async category =>
                    {
                        var authResult = await authService.AuthorizeAsync(user, category, new SameOwnerRequirement());
                        if (!authResult.Succeeded)
                        {
                            return Results.Forbid();
                        }

                        return useCases.UpdateCategory(dto).Match(
                            _ => Results.NoContent(),
                            error => Results.Problem(detail: error, statusCode: 400)
                        );
                    },
                    () => Task.FromResult(Results.NotFound())
                );
            })
            .WithSummary("Aktualizuje kategorię")
            .WithDescription("Zmienia nazwę istniejącej kategorii (wymagany właściciel lub Admin).")
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces(StatusCodes.Status404NotFound);

            // Usuwanie z weryfikacją właściciela zasobu
            categoriesGroup.MapDelete("{id:guid}", async (
                Guid id,
                ICategoryQueries queries,
                ICategoryUseCases useCases,
                IAuthorizationService authService,
                ClaimsPrincipal user) =>
            {
                var categoryOption = queries.GetCategory(id);

                return await categoryOption.Match(
                    async category =>
                    {
                        var authResult = await authService.AuthorizeAsync(user, category, new SameOwnerRequirement());
                        if (!authResult.Succeeded)
                        {
                            return Results.Forbid();
                        }

                        return useCases.RemoveCategory(id).Match(
                            _ => Results.NoContent(),
                            error => Results.Problem(detail: error, statusCode: 400)
                        );
                    },
                    () => Task.FromResult(Results.NotFound())
                );
            })
            .WithSummary("Usuwa kategorię")
            .WithDescription("Usuwa kategorię (wymagany właściciel lub Admin).")
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status403Forbidden)
            .Produces(StatusCodes.Status404NotFound);


            //ENDPOINTY Kontaktów
            var contactsGroup = app.MapGroup("/api/contacts")
                .WithTags("Kontakty")
                .RequireAuthorization();

            contactsGroup.MapPost("",
                (AddContactDto dto, IContactUseCases useCases, ClaimsPrincipal user) =>
                {
                    var userId = Guid.Parse(user.FindFirstValue(JwtRegisteredClaimNames.Sub)!);
                    return useCases.AddContact(userId, dto)
                        .Match(
                            result => Results.Created($"/api/contacts/{result.Id}", result),
                            error => Results.Problem(detail: error, statusCode: 400)
                        );
                }
            )
            .WithSummary("Dodaje nowy kontakt zalogowanego użytkownika")
            .WithDescription("Tworzy podstawowy profil kontaktu (imię, nazwisko, płeć, wiek).")
            .Produces<AddContactResult>(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status400BadRequest);

            contactsGroup.MapPut("",
                (UpdateContactDto dto, IContactUseCases useCases) =>
                useCases.UpdateContact(dto)
                    .Match(
                        _ => Results.NoContent(),
                        error => Results.Problem(detail: error, statusCode: 400)
                    )
            )
            .WithSummary("Aktualizuje dane kontaktu")
            .WithDescription("Modyfikuje dane osobowe wybranego kontaktu.")
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status400BadRequest);

            contactsGroup.MapDelete("{id:guid}",
                (Guid id, IContactUseCases useCases) =>
                useCases.RemoveContact(id)
                    .Match(
                        _ => Results.NoContent(),
                        error => Results.Problem(detail: error, statusCode: 400)
                    )
            )
            .WithSummary("Usuwa kontakt")
            .WithDescription("Usuwa kontakt oraz powiązane z nim dane (e-maile, telefony).")
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status400BadRequest);


            //ENDPOINTY Email (w kontaktach)
            app.MapPost("/api/contacts/emails",
                (AddEmailDto dto, IContactUseCases useCases) =>
                useCases.AddEmail(dto)
                    .Match(
                        _ => Results.Ok(),
                        error => Results.Problem(detail: error, statusCode: 400)
                    )
            )
            .WithSummary("Dodaje adres e-mail do kontaktu")
            .WithDescription("Przypisuje nowy adres e-mail powiązany z wybraną kategorią do wskazanego kontaktu.")
            .WithTags("Kontakty - E-maile")
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest);

            app.MapPost("/api/contacts/emails/remove",
                (RemoveEmailDto dto, IContactUseCases useCases) =>
                useCases.RemoveEmail(dto)
                    .Match(
                        _ => Results.NoContent(),
                        error => Results.Problem(detail: error, statusCode: 400)
                    )
            )
            .WithSummary("Usuwa adres e-mail z kontaktu")
            .WithDescription("Usuwa podany adres e-mail z listy adresów danego kontaktu.")
            .WithTags("Kontakty - E-maile")
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status400BadRequest);


            //ENDPOINTY Telefonów (w kontaktach)
            app.MapPost("/api/contacts/phones",
                (AddPhoneDto dto, IContactUseCases useCases) =>
                useCases.AddPhone(dto)
                    .Match(
                        _ => Results.Ok(),
                        error => Results.Problem(detail: error, statusCode: 400)
                    )
            )
            .WithSummary("Dodaje numer telefonu do kontaktu")
            .WithDescription("Przypisuje nowy numer telefonu do wybranego kontaktu.")
            .WithTags("Kontakty - Telefony")
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest);

            app.MapPost("/api/contacts/phones/remove",
                (RemovePhoneDto dto, IContactUseCases useCases) =>
                useCases.RemovePhone(dto)
                    .Match(
                        _ => Results.NoContent(),
                        error => Results.Problem(detail: error, statusCode: 400)
                    )
            )
            .WithSummary("Usuwa numer telefonu z kontaktu")
            .WithDescription("Usuwa wybrany numer telefonu z profilu kontaktu.")
            .WithTags("Kontakty - Telefony")
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status400BadRequest);

            //ENDPOINTY W USERZE
            var usersGroup = app.MapGroup("/api/users")
                .WithTags("Użytkownicy");

            // Rejestracja - dostępna bez tokena
            usersGroup.MapPost("", (AddUserDto dto, IUserUseCases userUseCases) =>
                userUseCases
                    .AddUser(dto)
                    .Match(
                        user => Results.Created($"/api/users/{user.Id}", user),
                        error => Results.Problem(detail: error, statusCode: 400)
                    )
            )
            .WithSummary("Rejestruje nowego użytkownika");

            // Pobieranie listy użytkowników - Tylko dla Administratora
            usersGroup.MapGet("", (IUserUseCases userUseCases) =>
                Results.Ok(userUseCases.GetUsers())
            )
            .RequireAuthorization("AdminOnly")
            .WithSummary("Pobiera listę wszystkich użytkowników (Tylko Admin)")
            .Produces<IEnumerable<UserDto>>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden);

            // Usuwanie użytkownika - Tylko dla Administratora
            usersGroup.MapDelete("{id:guid}", (Guid id, IUserUseCases useCases) =>
                useCases.RemoveUser(id)
                    .Match(
                        _ => Results.NoContent(),
                        error => Results.Problem(detail: error, statusCode: 400)
                    )
            )
            .RequireAuthorization("AdminOnly")
            .WithSummary("Usuwa użytkownika (Tylko Admin)")
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden);

            // Aktualizacja własnego profilu - Dla każdego zalogowanego użytkownika
            usersGroup.MapPut("profile", (UserProfileDto dto, IUserUseCases userUseCases, ClaimsPrincipal user) =>
            {
                var userId = Guid.Parse(user.FindFirstValue(JwtRegisteredClaimNames.Sub)!);
                return userUseCases.UpdateProfile(userId, dto)
                    .Match(
                        _ => Results.NoContent(),
                        error => Results.Problem(detail: error, statusCode: 400)
                    );
            })
            .RequireAuthorization()
            .WithSummary("Aktualizuje profil zalogowanego użytkownika")
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized);

            usersGroup.MapPut("{id:guid}/role", (Guid id, ChangeUserRoleDto dto, IUserUseCases userUseCases) =>
                userUseCases.ChangeRole(id, dto.Role)
                    .Match(
                        _ => Results.NoContent(),
                        error => Results.Problem(detail: error, statusCode: 400)
                    )
            )
            .RequireAuthorization("AdminOnly")
            .WithSummary("Zmienia rolę użytkownika (Tylko Admin)")
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden);

            // ENDPOINTY LOGOWANIA I ODŚWIEŻANIA TOKENA
            app.MapPost("/api/login", (LoginUserDto dto, IUserUseCases userUseCases) =>
                userUseCases.Login(dto).Match(
                    loginData => Results.Ok(new { loginData.AccessToken, loginData.RefreshToken }),
                    _ => Results.Unauthorized()
                )
            )
            .WithSummary("Logowanie użytkownika")
            .WithTags("Użytkownicy");

            app.MapPost("/api/refresh", (RefreshTokenDto dto, IUserUseCases userUseCases) =>
                userUseCases.RefreshToken(dto).Match(
                    loginData => Results.Ok(new { loginData.AccessToken, loginData.RefreshToken }),
                    error => Results.Problem(detail: error, statusCode: 401)
                )
            )
            .WithSummary("Odświeżanie tokenu dostępowego za pomocą Refresh Tokena")
            .WithTags("Użytkownicy");


            // Uruchomienie aplikacji
            app.Run();

        }
    }
}

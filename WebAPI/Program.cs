using Application.DomainServices;
using Application.OutputPorts;
using Application.Queries;
using Application.Queries.Dtos;
using Application.Repositories;
using Application.Services;
using Application.UseCases;
using Application.UseCases.Dtos;
using Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using WebAPI.Helpers;


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
            builder.Services.AddScoped<CategoryUniquenessChecker>();

            builder.Services.AddScoped<IUserRepository, UserRepository>();
            builder.Services.AddScoped<IUserUseCases, UserServices>();

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
            builder.Services.AddAuthorization();

            builder.Services.AddScoped<ITokenGenerator, TokenGenerator>();


            // Dodanie usług koniecznych dla OpenApi
            builder.Services.AddOpenApi();
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
                .WithTags("Kategorie");

            categoriesGroup.MapGet("",
                (ICategoryQueries queries) => Results.Ok(queries.GetCategories())
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
                (AddCategoryDto dto, ICategoryUseCases useCases) =>
                useCases.AddCategory(dto)
                    .Match(
                        category => Results.Created(
                            $"/api/categories/{category.Id}",
                            category),
                        error => Results.Problem(detail: error, statusCode: 400)
                    )
            )
            .WithSummary("Dodaje kategorię")
            .WithDescription("Tworzy nową kategorię o unikalnej nazwie i zwraca utworzony obiekt.")
            .Produces<AddCategoryResult>(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status400BadRequest);

            categoriesGroup.MapPut("",
                (UpdateCategoryDto dto, ICategoryUseCases useCases) =>
                useCases.UpdateCategory(dto)
                    .Match(
                        _ => Results.NoContent(),
                        error => Results.Problem(detail: error, statusCode: 400)
                    )
            )
            .WithSummary("Aktualizuje kategorię")
            .WithDescription("Zmienia nazwę istniejącej kategorii.")
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status400BadRequest);

            categoriesGroup.MapDelete("{id:guid}",
                (Guid id, ICategoryUseCases useCases) =>
                useCases.RemoveCategory(id)
                    .Match(
                        _ => Results.NoContent(),
                        error => Results.Problem(detail: error, statusCode: 400)
                    )
            )
            .WithSummary("Usuwa kategorię")
            .WithDescription("Usuwa kategorię, o ile nie jest ona aktualnie przypisana do żadnego adresu e-mail w kontaktach.")
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status400BadRequest);


            //ENDPOINTY Kontaktów
            app.MapPost("/api/contacts",
                (AddContactDto dto, IContactUseCases useCases) =>
                useCases.AddContact(dto)
                    .Match(
                        result => Results.Created($"/api/contacts/{result.Id}", result),
                        error => Results.Problem(detail: error, statusCode: 400)
                    )
            )
            .WithSummary("Dodaje nowy kontakt")
            .WithDescription("Tworzy podstawowy profil kontaktu (imię, nazwisko, płeć, wiek).")
            .WithTags("Kontakty")
            .Produces<AddContactResult>(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status400BadRequest);

            app.MapPut("/api/contacts",
                (UpdateContactDto dto, IContactUseCases useCases) =>
                useCases.UpdateContact(dto)
                    .Match(
                        _ => Results.NoContent(),
                        error => Results.Problem(detail: error, statusCode: 400)
                    )
            )
            .WithSummary("Aktualizuje dane kontaktu")
            .WithDescription("Modyfikuje dane osobowe wybranego kontaktu.")
            .WithTags("Kontakty")
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status400BadRequest);

            app.MapDelete("/api/contacts/{id:guid}",
                (Guid id, IContactUseCases useCases) =>
                useCases.RemoveContact(id)
                    .Match(
                        _ => Results.NoContent(),
                        error => Results.Problem(detail: error, statusCode: 400)
                    )
            )
            .WithSummary("Usuwa kontakt")
            .WithDescription("Usuwa kontakt oraz powiązane z nim dane (e-maile, telefony).")
            .WithTags("Kontakty")
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
            app.MapPost("/api/users", (AddUserDto dto, IUserUseCases userUseCases) =>
                userUseCases
                    .AddUser(dto)
                    .Match(
                        user => Results.Created($"/api/users/{user.Id}", user),
                        error => Results.Problem(detail: error, statusCode: 400)
                    )
            );

            //ENDPOINTY LOGOWANIA
            app.MapPost("/api/login", (LoginUserDto dto, IUserUseCases userUseCases) =>
                userUseCases.Login(dto).Match(
                    loginData => Results.Ok(new { loginData.AccessToken }),
                    _ => Results.Unauthorized()
                )
            );


            // Uruchomienie aplikacji
            app.Run();

        }
    }
}

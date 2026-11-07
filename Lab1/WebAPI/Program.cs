using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

using Application.DomainServices;
using Application.OutputPorts;
using Application.Queries;
using Application.Queries.Dtos;
using Application.Repositories;
using Application.Services;
using Application.UseCases;
using Application.UseCases.Dtos;
using WebAPI.Helpers;
using WebAPI.Inputs;
using Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(x => {
	x.TokenValidationParameters = new TokenValidationParameters{
		IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["ApplicationSettings:JWT_KEY"]!)),
		ValidateIssuer = false,
		ValidateAudience = false,
		ValidateLifetime = false,
		NameClaimType = "name",
		RoleClaimType = "role"	
	};
	x.MapInboundClaims = false;
});
builder.Services.AddAuthorization();

builder.Services.AddDbContext<AppDbContext>(options => 
	options.UseSqlite(builder.Configuration.GetConnectionString("default"))
);

builder.Services.AddScoped<ITokenGenerator, TokenGenerator>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<ICategoryQueries, CategoryQueries>();
builder.Services.AddScoped<IContactQueries, ContactQueries>();
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<IContactRepository, ContactRepository>();
builder.Services.AddScoped<ICategoryUseCases, CategoryServices>();
builder.Services.AddScoped<IContactUseCases, ContactServices>();
builder.Services.AddScoped<CategoryUniquenessChecker>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUserUseCases, UserServices>();

builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<WebAPI.GlobalExceptionHandler>();

builder.Services.AddOpenApi(options => {
	options.AddDocumentTransformer((document, context, ct) => {
		document.Components ??= new OpenApiComponents();
		document.Components.SecuritySchemes ??= new Dictionary<string, IOpenApiSecurityScheme>();
		document.Components.SecuritySchemes["Bearer"] = new OpenApiSecurityScheme{
			Type = SecuritySchemeType.Http,
			Scheme = "bearer",
			BearerFormat = "JWT",
			Description = "Wklej token JWT otrzymany z /api/login"
		};
		document.Security = [
			new OpenApiSecurityRequirement {
				[new OpenApiSecuritySchemeReference("Bearer", document)] = []
			}
		];
		return Task.CompletedTask;
	});
});

builder.Services.AddAuthorizationBuilder().AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));

var app = builder.Build();

if(app.Environment.IsDevelopment()){
	app.MapOpenApi();

	app.UseSwaggerUI(options => {
		options.SwaggerEndpoint("/openapi/v1.json", "API v1");
	});
}

app.UseExceptionHandler();
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

// Categories //
var categoriesGroup = app.MapGroup("/api/categories").WithTags("Kategorie");

categoriesGroup.MapGet("", (ICategoryQueries queries, ClaimsPrincipal user) => 
		Results.Ok(queries.GetCategories(Guid.Parse(user.FindFirstValue(JwtRegisteredClaimNames.Sub)!))))
.WithSummary("Pobiera listę kategorii")
.WithDescription("Zwraca kolekcję wszystkich dostępnych kategorii w systemie.")
.Produces<IEnumerable<CategoryDto>>(StatusCodes.Status200OK)
.RequireAuthorization();

categoriesGroup.MapGet("/{id:guid}", (Guid id, ICategoryQueries queries) => 
    queries.GetCategory(id).Match(
        category => Results.Ok(category),
        () => Results.NotFound()
    )
)
.WithSummary("Pobiera kategorię po ID")
.WithDescription("Zwraca szczegóły pojedynczej kategorii na podstawie jej unikalnego identyfikatora.")
.Produces<CategoryDto>(StatusCodes.Status200OK)
.Produces(StatusCodes.Status404NotFound);

categoriesGroup.MapPost("", (AddCategoryRequest input, ICategoryUseCases useCases, ClaimsPrincipal user) =>
	useCases.AddCategory(new AddCategoryDto(
		input.Name,
		Guid.Parse(user.FindFirstValue(JwtRegisteredClaimNames.Sub)!))
	).Match(category =>
		Results.Created($"/api/categories/{category.Id}", category),
		error => Results.BadRequest(error)
	)
)
.WithSummary("Dodaje kategorię")
.WithDescription("Tworzy nową kategorię i zwraca utworzony obiekt.")
.Produces<AddCategoryResult>(StatusCodes.Status201Created)
.Produces(StatusCodes.Status400BadRequest);

categoriesGroup.MapPut("", (UpdateCategoryDto dto, ICategoryUseCases useCases) => 
    useCases.UpdateCategory(dto).Match(
        _ => Results.NoContent(),
        error => Results.Problem(detail: error, statusCode: 400)
    )
)
.WithSummary("Aktualizuje kategorię")
.WithDescription("Modyfikuje dane istniejącej kategorii.")
.Produces(StatusCodes.Status204NoContent)
.Produces(StatusCodes.Status400BadRequest);

categoriesGroup.MapDelete("/{id:guid}", (Guid id, ICategoryUseCases useCases) => 
    useCases.RemoveCategory(id).Match(
        _ => Results.NoContent(),
        error => Results.Problem(detail: error, statusCode: 400)
    )
)
.WithSummary("Usuwa kategorię")
.WithDescription("Usuwa wskazaną kategorię na podstawie jej identyfikatora.")
.Produces(StatusCodes.Status204NoContent)
.Produces(StatusCodes.Status400BadRequest)
.RequireAuthorization("AdminOnly");

// Contacts //
var contactGroup = app.MapGroup("/api/contacts").WithTags("Kontakty");

contactGroup.MapGet("", (IContactQueries queries) => Results.Ok(queries.GetContacts()))
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
        error => Results.Problem(detail: error, statusCode: 400)
    )
)
.WithSummary("Dodaje kontakt")
.WithDescription("Tworzy nowy kontakt i zwraca utworzony obiekt.")
.Produces<AddContactResult>(StatusCodes.Status201Created)
.Produces(StatusCodes.Status400BadRequest);

contactGroup.MapPut("", (UpdateContactDto dto, IContactUseCases useCases) => 
    useCases.UpdateContact(dto).Match(
        _ => Results.NoContent(),
        error => Results.Problem(detail: error, statusCode: 400)
    )
)
.WithSummary("Aktualizuje kontakt")
.WithDescription("Modyfikuje dane istniejącego kontaktu.")
.Produces(StatusCodes.Status204NoContent)
.Produces(StatusCodes.Status400BadRequest);

contactGroup.MapDelete("/{id:guid}", (Guid id, IContactUseCases useCases) => 
    useCases.RemoveContact(id).Match(
        _ => Results.NoContent(),
        error => Results.Problem(detail: error, statusCode: 400)
    )
)
.WithSummary("Usuwa kontakt")
.WithDescription("Usuwa kontakt na podstawie identyfikatora.")
.Produces(StatusCodes.Status204NoContent)
.Produces(StatusCodes.Status400BadRequest);

// Contacts (Emails, Phones, Important Dates)
contactGroup.MapPost("/emails", (AddEmailDto dto, IContactUseCases useCases) => 
    useCases.AddEmail(dto).Match(
        _ => Results.NoContent(),
        error => Results.Problem(detail: error, statusCode: 400)
    )
)
.WithSummary("Dodaje e-mail do kontaktu")
.WithDescription("Przypisuje nowy adres e-mail do wskazanego kontaktu.")
.Produces(StatusCodes.Status204NoContent)
.Produces(StatusCodes.Status400BadRequest);

contactGroup.MapDelete("/emails", ([FromBody] RemoveEmailDto dto, IContactUseCases useCases) => 
    useCases.RemoveEmail(dto).Match(
        _ => Results.NoContent(),
        error => Results.Problem(detail: error, statusCode: 400)
    )
)
.WithSummary("Usuwa e-mail z kontaktu")
.WithDescription("Usuwa powiązany adres e-mail z kontaktu.")
.Produces(StatusCodes.Status204NoContent)
.Produces(StatusCodes.Status400BadRequest);

contactGroup.MapPost("/phones", (AddPhoneDto dto, IContactUseCases useCases) => 
    useCases.AddPhone(dto).Match(
        _ => Results.NoContent(),
        error => Results.Problem(detail: error, statusCode: 400)
    )
)
.WithSummary("Dodaje telefon do kontaktu")
.WithDescription("Przypisuje nowy numer telefonu do wskazanego kontaktu.")
.Produces(StatusCodes.Status204NoContent)
.Produces(StatusCodes.Status400BadRequest);

contactGroup.MapDelete("/phones", ([FromBody] RemovePhoneDto dto, IContactUseCases useCases) => 
    useCases.RemovePhone(dto).Match(
        _ => Results.NoContent(),
        error => Results.Problem(detail: error, statusCode: 400)
    )
)
.WithSummary("Usuwa telefon z kontaktu")
.WithDescription("Usuwa powiązany numer telefonu z kontaktu.")
.Produces(StatusCodes.Status204NoContent)
.Produces(StatusCodes.Status400BadRequest);

contactGroup.MapPost("/important-dates", (AddImportantDateDto dto, IContactUseCases useCases) => 
    useCases.AddImportantDate(dto).Match(
        _ => Results.NoContent(),
        error => Results.Problem(detail: error, statusCode: 400)
    )
)
.WithSummary("Dodaje ważną datę do kontaktu")
.WithDescription("Przypisuje nową ważną datę do wskazanego kontaktu.")
.Produces(StatusCodes.Status204NoContent)
.Produces(StatusCodes.Status400BadRequest);

contactGroup.MapDelete("/important-dates", ([FromBody] RemoveImportantDateDto dto, IContactUseCases useCases) => 
    useCases.RemoveImportantDate(dto).Match(
        _ => Results.NoContent(),
        error => Results.Problem(detail: error, statusCode: 400)
    )
)
.WithSummary("Usuwa ważną datę z kontaktu")
.WithDescription("Usuwa powiązaną ważną datę z kontaktu.")
.Produces(StatusCodes.Status204NoContent)
.Produces(StatusCodes.Status400BadRequest);

app.MapPost("/api/users", (AddUserDto dto, IUserUseCases userUseCases) =>
	userUseCases.AddUser(dto).Match(
			user => Results.Created($"/api/users/{user.Id}", user),
			error => Results.Problem(detail: error, statusCode: 400)
		)	
);

app.MapPost("/api/login", (LoginUserDto dto, IUserUseCases userUseCases) =>
	userUseCases.Login(dto).Match(
		loginData => Results.Ok(loginData.AccessToken),
		_ => Results.Unauthorized()
	)
);

app.Run();

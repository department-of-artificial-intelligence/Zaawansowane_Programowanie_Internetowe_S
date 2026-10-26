using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Application.DomainServices;
using Application.OutputPorts;
using Application.Queries;
using Application.Queries.Dtos;
using Application.Repositories;
using Application.Services;
using Application.UseCases;
using Application.UseCases.Dtos;
using Data;
using Microsoft.IdentityModel.Tokens.Experimental;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(options => 
	options.UseSqlite(builder.Configuration.GetConnectionString("default"))
);

builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<ICategoryQueries, CategoryQueries>();
builder.Services.AddScoped<IContactQueries, ContactQueries>();
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<IContactRepository, ContactRepository>();
builder.Services.AddScoped<ICategoryUseCases, CategoryServices>();
builder.Services.AddScoped<IContactUseCases, ContactServices>();
builder.Services.AddScoped<CategoryUniquenessChecker>();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(
    x => {x.TokenValidationParameters = new TokenValidationParameters
    {
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["ApplicationSetting:JWT_KEY"]!)),
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateLifetime = true,
        NameClaimType = "name",
        RoleClaimType = "role"
    };
    x.MapInboundClaims = false;
});
builder.Services.AddAuthorization();

builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<WebAPI.GlobalExceptionHandler>();

builder.Services.AddOpenApi();

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

categoriesGroup.MapGet("", (ICategoryQueries queries) => Results.Ok(queries.GetCategories()))
.RequireAuthorization()
.WithSummary("Pobiera listę kategorii")
.WithDescription("Zwraca kolekcję wszystkich dostępnych kategorii w systemie.")
.Produces<IEnumerable<CategoryDto>>(StatusCodes.Status200OK);

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

categoriesGroup.MapPost("", (AddCategoryDto dto, ICategoryUseCases useCases) => 
	useCases.AddCategory(dto).Match(
		category => Results.Created($"/api/categories/{category.Id}", category),
		error => Results.Problem(detail: error, statusCode: 400)
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
.Produces(StatusCodes.Status400BadRequest);

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

app.Run();

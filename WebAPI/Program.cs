// ============================================================
// ✅ USINGI – zawsze na samej górze pliku
// ============================================================
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using WebAPI.Data;        // 🔹 Twój AppDbContext
using WebAPI.Models;      // 🔹 Model User
using WebAPI.Services;    // 🔹 TokenGenerator

// ============================================================
// 🧱 Budowanie aplikacji
// ============================================================
var builder = WebApplication.CreateBuilder(args);

// ============================================================
// 1️⃣ Baza danych
// ============================================================
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// ============================================================
// 2️⃣ Autentykacja JWT
// ============================================================
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        var key = builder.Configuration["ApplicationSettings:JWT_KEY"];
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key!))
        };
    });

// ============================================================
// 3️⃣ Autoryzacja
// ============================================================
builder.Services.AddAuthorization();

// ============================================================
// 4️⃣ Swagger z obsługą JWT
// ============================================================
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(option =>
{
    option.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        Description = "Wpisz: Bearer {token}"
    });

    option.AddSecurityRequirement(new OpenApiSecurityRequirement
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
            new string[]{}
        }
    });
});

// ============================================================
// 5️⃣ Rejestracja serwisu TokenGenerator
// ============================================================
builder.Services.AddScoped<TokenGenerator>();

// ============================================================
// 🧱 Tworzenie aplikacji
// ============================================================
var app = builder.Build();

// ============================================================
// 6️⃣ Middleware – kolejność ma znaczenie!
// ============================================================
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

// ============================================================
// 7️⃣ Swagger tylko w trybie developerskim
// ============================================================
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// ============================================================
// 8️⃣ Przykładowe dane pogodowe
// ============================================================
var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild",
    "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

// ============================================================
// 9️⃣ Publiczny endpoint
// ============================================================
app.MapGet("/weatherforecast", () =>
{
    var forecast = Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();

    return forecast;
})
.WithName("GetWeatherForecast");

// ============================================================
// 🔒 Chroniony endpoint – wymaga JWT
// ============================================================
app.MapGet("/api/secret", () => "Tajne dane! Dostęp tylko z poprawnym tokenem 🔒")
   .RequireAuthorization();

// ============================================================
// 👤 Rejestracja użytkownika
// ============================================================
app.MapPost("/api/users/register", async (AppDbContext db, UserDTO dto) =>
{
    var hasher = new PasswordHasher<User>();
    var passwordHash = hasher.HashPassword(null!, dto.Password);

    var user = new User(dto.UserName, passwordHash, dto.FirstName, dto.LastName);
    db.Users.Add(user);
    await db.SaveChangesAsync();

    return Results.Ok(new { user.Id, user.UserName });
});

record UserDTO(string UserName, string Password, string FirstName, string LastName);

// ============================================================
// 🔑 Logowanie – generuje token JWT
// ============================================================
app.MapPost("/api/users/login", async (AppDbContext db, IConfiguration config, LoginDTO dto) =>
{
    var user = await db.Users.FirstOrDefaultAsync(u => u.UserName == dto.UserName);
    if (user == null) return Results.Unauthorized();

    var hasher = new PasswordHasher<User>();
    var result = hasher.VerifyHashedPassword(user, user.PasswordHash, dto.Password);
    if (result == PasswordVerificationResult.Failed) return Results.Unauthorized();

    var tokenGen = new TokenGenerator(config["ApplicationSettings:JWT_KEY"]!);
    var token = tokenGen.GenerateToken(user);

    return Results.Ok(new { token });
});

record LoginDTO(string UserName, string Password);

// ============================================================
// 👤 Endpoint profilu – wymaga autoryzacji
// ============================================================
app.MapGet("/api/profile", async (HttpContext http, AppDbContext db) =>
{
    var id = int.Parse(http.User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)!.Value);
    var user = await db.Users.FindAsync(id);
    return Results.Ok(user);
}).RequireAuthorization();

// ============================================================
// 📘 Pomocniczy rekord – prognoza pogody
// ============================================================
record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}

// ============================================================
// 🏁 Uruchomienie aplikacji
// ============================================================
app.Run();



using Microsoft.AspNetCore.Identity;

using Application.OutputPorts;

namespace WebAPI.Helpers;

// Zadanie 7 - adapter na klasę PasswordHasher<TUser> z ASP.NET Core Identity.
// Pierwszy parametr jej metod nie jest używany, dlatego przekazujemy null.
public class PasswordHasher : IPasswordHasher
{
    private readonly PasswordHasher<object> hasher = new();

    public string Hash(string password) =>
        hasher.HashPassword(null!, password);

    public bool Verify(string passwordHash, string password) =>
        hasher.VerifyHashedPassword(null!, passwordHash, password)
            is PasswordVerificationResult.Success
            or PasswordVerificationResult.SuccessRehashNeeded;
}

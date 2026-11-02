using Microsoft.AspNetCore.Identity;
namespace WebAPI.Helpers;

// Zadanie 7: implementacja portu IPasswordHasher oparta o PasswordHasher<TUser>.
// Domena nie wie nic o Microsoft.AspNetCore.Identity (tak jak o JWT w TokenGenerator).
public class IdentityPasswordHasher : Application.OutputPorts.IPasswordHasher
{
    // Parametr TUser nie jest używany wewnątrz metod, więc podajemy null (uwaga w PDF)
    private readonly PasswordHasher<object> hasher = new();

    public string Hash(string password) =>
        hasher.HashPassword(null!, password);

    public bool Verify(string passwordHash, string password) =>
        hasher.VerifyHashedPassword(null!, passwordHash, password)
            != PasswordVerificationResult.Failed;
}

using Application.OutputPorts;
using Microsoft.AspNetCore.Identity;

namespace WebAPI.Helpers;

public class PasswordHasherAdapter : IPasswordHasher
{
    private readonly PasswordHasher<object> hasher = new();

    public string Hash(string password) => hasher.HashPassword(null!, password);

    public bool Verify(string hashedPassword, string password) =>
        hasher.VerifyHashedPassword(null!, hashedPassword, password)
        != PasswordVerificationResult.Failed;
}

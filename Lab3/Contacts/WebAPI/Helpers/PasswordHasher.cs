using Microsoft.AspNetCore.Identity;

using Application.Domain;
using Application.OutputPorts;

namespace WebAPI.Helpers;

public class PasswordHasher : IPasswordHasher
{

    private readonly PasswordHasher<User> hasher = new();

    public string Hash(string password) =>
        hasher.HashPassword(null!, password);

    public bool Verify(string passwordHash, string password) =>
        hasher.VerifyHashedPassword(null!, passwordHash, password)
            != PasswordVerificationResult.Failed;
}

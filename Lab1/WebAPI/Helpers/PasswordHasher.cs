using Application.Domain;
using Application.OutputPorts;
using Microsoft.AspNetCore.Identity;

namespace WebAPI.Helpers;

public class PasswordHasher : IPasswordHasher
{
    private readonly PasswordHasher<User> _hasher = new();

    public string HashPassword(string password) => _hasher.HashPassword(null!, password);

    public bool VerifyPassword(string hashedPassword, string providedPassword) =>
        _hasher.VerifyHashedPassword(null!, hashedPassword, providedPassword) != PasswordVerificationResult.Failed;
}
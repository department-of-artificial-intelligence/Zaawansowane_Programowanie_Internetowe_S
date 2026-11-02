using Application.OutputPorts;
using Microsoft.AspNetCore.Identity;

namespace WebAPI.Helpers;

public class PasswordHasher : IPasswordHasher
{
    private readonly PasswordHasher<object> _hasher = new();

    public string HashPassword(string password) =>
        _hasher.HashPassword(null!, password);

    public bool VerifyHashedPassword(string hashedPassword, string providedPassword) =>
        _hasher.VerifyHashedPassword(null!, hashedPassword, providedPassword) 
            != PasswordVerificationResult.Failed;
}
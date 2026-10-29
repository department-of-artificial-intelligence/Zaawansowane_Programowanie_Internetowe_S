using Microsoft.AspNetCore.Identity;
using Application.OutputPorts;


namespace WebAPI.Helpers
{
    public class PasswordHasher : IPasswordHasher
    {
        private readonly PasswordHasher<object> _hasher = new();

        public string HashPassword(string password) =>
            _hasher.HashPassword(null!, password);

        public bool VerifyHashedPassword(string hashedPassword, string providedPassword)
        {
            var result = _hasher.VerifyHashedPassword(null!, hashedPassword, providedPassword);
            return result != PasswordVerificationResult.Failed;
        }
    }
}

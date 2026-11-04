using Application.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Application.Domain
{
    public class User : Entity
    {
        public string UserName { get; private set; } = string.Empty;
        public string Password { get; private set; } = string.Empty;
        public UserRole Role { get; private set; } = UserRole.User;
        public UserProfile? Profile { get; private set; }

        public string? RefreshToken { get; private set; }
        public DateTime? RefreshTokenExpiryTime { get; private set; }

        private User() : base(Guid.Empty) { }

        private User(Guid id, string userName, string password, UserRole role, UserProfile? profile = null) : base(id)
        {
            UserName = userName;
            Password = password;
            Role = role;
            Profile = profile;
        }
        public Result<Unit> SetPassword(string password) =>
             Validators.NotEmpty(password, "Hasło nie może być puste")
             .Bind(_ => password.Length < 6
                 ? Result.Failure<Unit>("Hasło musi mieć co najmniej 6 znaków")
                 : Result.Success(Unit.Instance))
             .Tap(_ => Password = password);

        public Result<Unit> UpdateProfile(UserProfile profile)
        {
            Profile = profile;
            return Result.Success(Unit.Instance);
        }
        public static Result<User> Create(
        string userName,
        string password,
        UserRole role = UserRole.User,
        UserProfile? profile = null) =>
        Validators.NotEmpty(userName, "Nazwa użytkownika nie może być pusta")
        .Bind(_ => Validators.NotEmpty(password, "Hasło nie może być puste"))
        .Bind(_ => password.Length < 6
            ? Result.Failure<Unit>("Hasło musi składać się z co najmniej 6 znaków")
            : Result.Success(Unit.Instance))
        .Map(_ => new User(Guid.NewGuid(), userName, password, role, profile));

        public Result<Unit> ChangeRole(UserRole newRole)
        {
            Role = newRole;
            return Result.Success(Unit.Instance);
        }        

        public void SetRefreshToken(string token, DateTime expiryTime)
        {
            RefreshToken = token;
            RefreshTokenExpiryTime = expiryTime;
        }

        public bool IsRefreshTokenValid(string token) =>
            RefreshToken == token && RefreshTokenExpiryTime.HasValue && RefreshTokenExpiryTime > DateTime.UtcNow;
    }

}

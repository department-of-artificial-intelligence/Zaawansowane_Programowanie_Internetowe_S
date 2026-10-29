using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Utilities;

namespace Application.Domain
{
    public class User : Entity
    {
        public string UserName { get; private set; } = string.Empty;
        public string Password { get; private set; } = string.Empty;

        public UserRole Role { get; private set; } = UserRole.User;

        public void ChangeRole(UserRole newRole)
        {
            Role = newRole;
        }

        // Pola profilu użytkownika (Zadanie 4)
        public string? FirstName { get; private set; }
        public string? LastName { get; private set; }
        public DateOnly? DateOfBirth { get; private set; }
        public string? Address { get; private set; }
        public string? RefreshToken { get; private set; }
        public DateTime? RefreshTokenExpiryTime { get; private set; }


        // Prywatny konstruktor przyjmuje od razu kompletny, poprawny stan encji
        private User(Guid id, string userName, string password) : base(id)
        {
            UserName = userName;
            Password = password;
        }

        public void SetRefreshToken(string token, DateTime expiryTime)
        {
            RefreshToken = token;
            RefreshTokenExpiryTime = expiryTime;
        }

        public void RevokeRefreshToken()
        {
            RefreshToken = null;
            RefreshTokenExpiryTime = null;
        }

        // Walidacja hasła wykorzystywana zarówno w Create, jak i SetPassword
        private static Result<Unit> ValidatePassword(string password)
        {
            if (string.IsNullOrWhiteSpace(password))
                return Result.Failure<Unit>("Hasło nie może być puste");

            if (password.Length < 6)
                return Result.Failure<Unit>("Hasło musi mieć co najmniej 6 znaków");

            return Result.Success(Unit.Instance);
        }

        // Metoda zmiany hasła chroni niezmiennik – nie pozwoli przypisać niepoprawnego hasła
        public Result<Unit> SetPassword(string password) =>
            ValidatePassword(password)
                .Tap(_ => Password = password);

        // Metoda domenowa do zmiany danych profilowych
        public void UpdateProfile(string? firstName, string? lastName, DateOnly? dateOfBirth, string? address)
        {
            FirstName = firstName;
            LastName = lastName;
            DateOfBirth = dateOfBirth;
            Address = address;
        }

        // Smart konstruktor waliduje jednocześnie login i hasło
        public static Result<User> Create(string userName, string password) =>
            Validators
                .NotEmpty(userName, "Nazwa użytkownika nie może być pusta")
                .Bind(_ => ValidatePassword(password))
                .Map(_ => new User(Guid.NewGuid(), userName, password));
    }
}

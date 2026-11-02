using Application.OutputPorts;
using Application.Utilities;

namespace Application.Domain;

public class User : Entity
{
    public const int MaxUserNameChars = 50;

    public string UserName { get; private set; } = string.Empty;

    // Zadanie 7: w bazie trzymamy wyłącznie skrót hasła, nigdy samo hasło
    public string PasswordHash { get; private set; } = string.Empty;

    public UserRole Role { get; private set; } = UserRole.User;

    // Zadanie 4
    public UserProfile Profile { get; private set; } = UserProfile.Empty;

    // Zadanie 10
    public string? RefreshTokenHash { get; private set; }
    public DateTime? RefreshTokenExpiresAt { get; private set; }

    private User(Guid id, string userName, string passwordHash) : base(id)
    {
        UserName = userName;
        PasswordHash = passwordHash;
    }

    // Zadanie 3: hasło jest argumentem smart konstruktora, więc nie da się
    // utworzyć użytkownika bez poprawnego hasła (niezmiennik "użytkownik ma hasło")
    public static Result<User> Create(string userName, string password, IPasswordHasher hasher) =>
        Validators.NotEmpty(userName, "Nazwa użytkownika nie może być pusta")
            .Bind(_ => Validators.MaxLength(userName.Trim(), MaxUserNameChars,
                $"Nazwa użytkownika może mieć maksymalnie {MaxUserNameChars} znaków."))
            .Bind(_ => PasswordPolicy.Validate(password))
            .Map(_ => new User(Guid.NewGuid(), userName.Trim(), hasher.Hash(password)));

    // Zadanie 3: SetPassword waliduje nowe hasło
    public Result<Unit> SetPassword(string password, IPasswordHasher hasher) =>
        PasswordPolicy.Validate(password)
            .Tap(_ => PasswordHash = hasher.Hash(password));

    public bool VerifyPassword(string password, IPasswordHasher hasher) =>
        hasher.Verify(PasswordHash, password);

    // Zadanie 6
    public void ChangeRole(UserRole role) => Role = role;

    // Zadanie 4
    public void UpdateProfile(UserProfile profile) => Profile = profile;

    // Zadanie 10
    public void SetRefreshToken(string refreshTokenHash, DateTime expiresAt)
    {
        RefreshTokenHash = refreshTokenHash;
        RefreshTokenExpiresAt = expiresAt;
    }

    public void RevokeRefreshToken()
    {
        RefreshTokenHash = null;
        RefreshTokenExpiresAt = null;
    }

    public bool HasValidRefreshToken(DateTime now) =>
        RefreshTokenHash is not null && RefreshTokenExpiresAt > now;
}

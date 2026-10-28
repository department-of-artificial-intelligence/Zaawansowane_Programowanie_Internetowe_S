using Application.OutputPorts;
using Application.Utilities;

namespace Application.Domain;

public class User : Entity
{
    public string UserName { get; private set; }

    // Zadanie 7 - w bazie przechowywany jest wyłącznie skrót hasła.
    public string PasswordHash { get; private set; }

    public UserRole Role { get; private set; } = UserRole.User;

    // Zadanie 4 - profil użytkownika.
    public UserProfile Profile { get; private set; } = UserProfile.Empty;

    // Zadanie 10 - skrót aktualnego refresh tokenu i data jego wygaśnięcia.
    public string? RefreshTokenHash { get; private set; }
    public DateTime? RefreshTokenExpiresAt { get; private set; }

    private User(Guid id, string userName, string passwordHash) : base(id)
    {
        UserName = userName;
        PasswordHash = passwordHash;
    }

    // Zadanie 3 - hasło jest walidowane przy każdej zmianie.
    public Result<Unit> SetPassword(string password, IPasswordHasher passwordHasher) =>
        PasswordPolicy
            .Validate(password)
            .Tap(_ => PasswordHash = passwordHasher.Hash(password));

    public bool VerifyPassword(string password, IPasswordHasher passwordHasher) =>
        passwordHasher.Verify(PasswordHash, password);

    public void ChangeRole(UserRole role) => Role = role;

    public void UpdateProfile(UserProfile profile) => Profile = profile;

    public void IssueRefreshToken(string tokenHash, DateTime expiresAt)
    {
        RefreshTokenHash = tokenHash;
        RefreshTokenExpiresAt = expiresAt;
    }

    public void RevokeRefreshToken()
    {
        RefreshTokenHash = null;
        RefreshTokenExpiresAt = null;
    }

    public bool HasValidRefreshToken(string tokenHash, DateTime now) =>
        RefreshTokenHash == tokenHash && RefreshTokenExpiresAt > now;

    // Zadanie 3 - hasło jest argumentem smart konstruktora, dzięki czemu
    // nie da się utworzyć użytkownika bez poprawnego hasła.
    public static Result<User> Create(
        string userName,
        string password,
        IPasswordHasher passwordHasher) =>
        Result.Combine(
                UserNamePolicy.Validate(userName),
                PasswordPolicy.Validate(password))
            .Map(_ => new User(
                Guid.NewGuid(),
                userName.Trim(),
                passwordHasher.Hash(password)));
}

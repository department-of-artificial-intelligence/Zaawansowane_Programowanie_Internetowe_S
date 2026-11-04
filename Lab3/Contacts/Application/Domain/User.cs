using Application.Utilities;

namespace Application.Domain;

public class User : Entity
{
    public string UserName { get; private set; } = string.Empty;

    public string Password { get; private set; } = string.Empty;

    public UserRole Role { get; private set; } = UserRole.User;

    public UserProfile Profile { get; private set; } = UserProfile.Empty;

    public string RefreshToken { get; private set; } = string.Empty;

    public DateTime RefreshTokenExpiresAt { get; private set; }

    private User(Guid id, string userName, string password) : base(id)
    {
        UserName = userName;
        Password = password;
    }

    public void ChangeRole(UserRole role)
    {
        Role = role;
    }

    public void SetProfile(UserProfile profile)
    {
        Profile = profile;
    }

    public void SetRefreshToken(string refreshToken, DateTime expiresAt)
    {
        RefreshToken = refreshToken;
        RefreshTokenExpiresAt = expiresAt;
    }

    private static Result<Unit> ValidatePassword(string password) =>
        Validators
            .NotEmpty(password, "Hasło nie może być puste.")
            .Bind(_ => password.Length >= 8
                ? Result.Success(Unit.Instance)
                : Result.Failure<Unit>("Hasło musi mieć co najmniej 8 znaków."))
            .Bind(_ => password.Any(char.IsDigit)
                ? Result.Success(Unit.Instance)
                : Result.Failure<Unit>("Hasło musi zawierać co najmniej jedną cyfrę."));

    public static Result<User> Create(
        string userName,
        string password,
        Func<string, string> hashPassword) =>
        Validators
            .NotEmpty(userName, "Nazwa użytkownika nie może być pusta")
            .Bind(_ => ValidatePassword(password))
            .Map(_ => new User(Guid.NewGuid(), userName, hashPassword(password)));

}

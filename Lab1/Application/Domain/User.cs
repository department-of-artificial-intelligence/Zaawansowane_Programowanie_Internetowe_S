using Application.Utilities;

namespace Application.Domain;

public class User : Entity
{
    public string UserName { get; private set; } = string.Empty;
    public string Password { get; private set; } = string.Empty;
    public UserRole Role { get; private set; } = UserRole.User;
	public string? FirstName { get; private set; }
    public string? LastName { get; private set; }
    public DateOnly? BirthDate { get; private set; }
    public string? Address { get; private set; }
    public string? RefreshToken { get; private set; }
    public DateTime? RefreshTokenExpiryTime { get; private set; }

    private User(Guid id, string userName) : base(id)
    {
        UserName = userName;
    }

    public Result<User> SetPassword(string password) =>
        Validators.NotEmpty(password, "Hasło nie może być puste")
            .Bind(_ => password.Length >= 6 ? Result.Success(this)
                : Result.Failure<User>("Hasło musi mieć co najmniej 6 znaków"))
            .Tap(user => user.Password = password);

    public static Result<User> Create(string userName, string password) =>
        Validators.NotEmpty(userName, "Nazwa użytkownika nie może być pusta")
            .Map(_ => new User(Guid.NewGuid(), userName)).Bind(user => user.SetPassword(password));

	public void UpdateProfile(string? firstName, string? lastName, DateOnly? birthDate, string? address)
    {
        FirstName = firstName;
        LastName = lastName;
        BirthDate = birthDate;
        Address = address;
    }

    public void SetRole(UserRole role) => Role = role;

    public void SetRefreshToken(string refreshToken, DateTime expiryTime)
    {
        RefreshToken = refreshToken;
        RefreshTokenExpiryTime = expiryTime;
    }
}
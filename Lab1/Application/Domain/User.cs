using Application.Utilities;
namespace Application.Domain;
public class User : Entity
{
public string UserName {get; private set;} = string.Empty;
public string Password {get;private set;} = string.Empty;
public UserRole Role { get; private set; } = UserRole.User;
public string FirstName { get; private set; } = string.Empty;
public string LastName { get; private set; } = string.Empty;
public DateTime? BirthDate { get; private set; }
public string Address { get; private set; } = string.Empty;
public string? RefreshToken { get; private set; }
public DateTime? RefreshTokenExpiresAt { get; private set; }

private User(Guid id, string userName, string password) : base(id)
{
UserName = userName;
Password = password;
}

public Result<Unit> SetPassword(string password)
{
if (string.IsNullOrWhiteSpace(password))
    return Result.Failure<Unit>("Hasło nie może być puste.");
if (password.Length < 8)
    return Result.Failure<Unit>("Hasło musi mieć co najmniej 8 znaków.");
Password = password;
return Result.Success(Unit.Instance);
}
public Result<Unit> UpdateProfile(string firstName, string lastName, DateTime? birthDate, string address)
{
FirstName = firstName;
LastName = lastName;
BirthDate = birthDate;
Address = address;
return Result.Success(Unit.Instance);
}

public void SetRefreshToken(string token, DateTime expiresAt)
{
RefreshToken = token;
RefreshTokenExpiresAt = expiresAt;
}

public void SetRole(UserRole role) => Role = role;

public static Result<User> Create(string userName, string password) =>
string.IsNullOrWhiteSpace(userName)
    ? Result.Failure<User>("Nazwa użytkownika nie może być pusta")
    : string.IsNullOrWhiteSpace(password)
        ? Result.Failure<User>("Hasło nie może być puste.")
        : password.Length < 8
            ? Result.Failure<User>("Hasło musi mieć co najmniej 8 znaków.")
            : Result.Success(new User(Guid.NewGuid(), userName, password));
}
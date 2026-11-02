using Application.Utilities;
using Application.OutputPorts;
namespace Application.Domain;
public class User : Entity
{
    public string UserName {get; private set;} = string.Empty;
    public string Password {get;private set;} = string.Empty;
    public UserRole Role {get; private set;} = UserRole.User;
    public FirstName FirstName {get; private set;} = null!;
    public LastName LastName {get; private set;} = null!;
    public DateTime? DateOfBirth {get; private set;}
    public string Address {get; private set;} = string.Empty;
    public string? RefreshToken { get; private set; }
    public DateTime? RefreshTokenExpiryTime { get; private set; }
    private User(Guid id, string userName, string password, FirstName firstName, LastName lastName, DateTime? dateOfBirth, string address) : base(id)
    {
        UserName = userName;
        Password = password;
        FirstName = firstName;
        LastName = lastName;
        DateOfBirth = dateOfBirth;
        Address = address;
    }
    private User() : base(Guid.Empty)
    {
        UserName = null!;
        Password = null!;
        FirstName = null!;
        LastName = null!;
        Address = null!;
    }
    public static Result<User> Create(string userName, string rawPassword,IPasswordHasher passwordHasher, FirstName firstName, LastName lastName, DateTime? dateOfBirth = null, string address = "") =>
        Validators
            .NotEmpty(userName, "Nazwa użytkownika nie może być pusta")
            .Bind(_=> ValidatePassword(rawPassword))
            .Map(_ => new User(Guid.NewGuid(), userName, passwordHasher.HashPassword(rawPassword), firstName, lastName, dateOfBirth, address));

    private static Result<Unit> ValidatePassword(string password)=>
        Validators
            .NotEmpty(password, "Hasło nie moe być puste")
            .Bind(_=>password.Length<6
                ? Result.Failure<Unit>("Hasło musi mieć min 6 znaków")
                : Result.Success(Unit.Instance));

    public void UpdateUser(FirstName firstName, LastName lastName, DateTime? dateOfBirth, string address)
    {
            FirstName = firstName;
            LastName = lastName;
            DateOfBirth = dateOfBirth;
            Address = address;
    }
    public void SetRole(UserRole role)
    {
        Role = role;
    }
    public void SetRefreshToken(string refreshToken, DateTime expiryTime)
    {
        RefreshToken = refreshToken;
        RefreshTokenExpiryTime = expiryTime;
    }

    public void RevokeRefreshToken()
    {
        RefreshToken = null;
        RefreshTokenExpiryTime = null;
    }
}
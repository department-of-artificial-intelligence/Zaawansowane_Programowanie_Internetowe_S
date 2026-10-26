using Application.Utilities;
namespace Application.Domain;
public class User : Entity
{
    public string UserName {get; private set;} = string.Empty;
    public string Password {get;private set;} = string.Empty;
    private User(Guid id, string userName) : base(id)
    {
    UserName = userName;
    }
    public void SetPassword(string password)
    {
    this.Password = password;
    }
    public static Result<User> Create(string userName) =>
        Validators
            .NotEmpty(userName, "Nazwa użytkownika nie może być pusta")
            .Map(_ => new User(Guid.NewGuid(), userName));
}
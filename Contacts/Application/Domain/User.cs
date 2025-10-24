namespace Contacts.Application.Domain;

public class User
{
    public int Id { get; private set; }
    public string UserName { get; private set; } = string.Empty;
    public string Password { get; private set; } = string.Empty;

    private User() { }

    public User(string userName)
    {
        this.UserName = userName;
    }

    public void SetPassword(string password)
    {
        this.Password = password;
    }
}

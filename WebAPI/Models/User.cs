public class User
{
    public int Id { get; private set; }
    public string UserName { get; private set; }
    public string PasswordHash { get; private set; }

    public string FirstName { get; private set; }
    public string LastName { get; private set; }
    public DateTime? BirthDate { get; private set; }
    public string Address { get; private set; }

    private User() { }

    public User(string username, string passwordHash, string firstName, string lastName)
    {
        UserName = username;
        PasswordHash = passwordHash;
        FirstName = firstName;
        LastName = lastName;
    }
}

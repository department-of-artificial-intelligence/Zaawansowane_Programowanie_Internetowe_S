namespace Contacts.Application.Domain;

public class Category(string name, int id)
{
    public Category(string name, User user)
        : this(name, 0) {
            User = user;
            UserId = user.Id;
        }

    public int Id { get; private set; } = id;
    public string Name { get; private set; } = name;
    
    public User? User {get; private set;}
    public int UserId {get; private set;}
}

using Contacts.Application.Domain;
using Contacts.Application.Repositories;

namespace Contacts.Data;

public class UserRepository(AppDbContext context) : IUserRepository
{
    public User? GetByName(string userName) =>
        context.Users.SingleOrDefault(user => user.UserName == userName);

    public bool UserNotExists(string userName) =>
        context.Users.SingleOrDefault(u => u.UserName == userName) is null;

    public void Add(User user)
    {
        context.Users.Add(user);
    }
}

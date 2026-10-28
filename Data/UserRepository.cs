using Application.Domain;
using Application.Repositories;
using Application.Utilities;

namespace Data;

public class UserRepository(AppDbContext context) : IUserRepository
{
    public Option<User> GetByName(string userName)
    {
        var user = context.Users.SingleOrDefault(
            user => user.UserName == userName);
        return
            user is not null
                ? Option.Some(user)
                : Option.None<User>();
    }

    public Option<User> GetById(Guid userId)
    {
        var user = context.Users.Find(userId);
        return
            user is not null
                ? Option.Some(user)
                : Option.None<User>();
    }

    public void Remove(User user)
    {
        context.Users.Remove(user);
    }

    public void Add(User user)
    {
        context.Users.Add(user);
    }
}
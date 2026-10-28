using Application.Domain;
using Application.Repositories;
using Application.Utilities;

namespace Data;

public class UserRepository(AppDbContext context) : IUserRepository
{
    public void Add(User user)
    {
        context.Users.Add(user);
    }

    public void Remove(User user)
    {
        context.Remove(user);
    }

    public Option<User> GetById(Guid userId)
    {
        var user = context.Users.Find(userId);
        return user is not null
            ? Option.Some(user)
            : Option.None<User>();
    }

    public Option<User> GetByName(string userName)
    {
        var user = context.Users.SingleOrDefault(
            user => user.UserName == userName);
        return user is not null
            ? Option.Some(user)
            : Option.None<User>();
    }

    public Option<User> GetByRefreshTokenHash(string refreshTokenHash)
    {
        var user = context.Users.SingleOrDefault(
            user => user.RefreshTokenHash == refreshTokenHash);
        return user is not null
            ? Option.Some(user)
            : Option.None<User>();
    }
}

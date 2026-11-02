using Application.Domain;
using Application.Repositories;
using Application.Utilities;
namespace Data;

public class UserRepository(AppDbContext context) : IUserRepository
{
    public void Add(User user) => context.Users.Add(user);

    public void Remove(User user) => context.Users.Remove(user);

    public Option<User> GetById(Guid userId) =>
        ToOption(context.Users.Find(userId));

    public Option<User> GetByName(string userName) =>
        ToOption(context.Users.SingleOrDefault(user => user.UserName == userName));

    public Option<User> GetByRefreshTokenHash(string refreshTokenHash) =>
        ToOption(context.Users.SingleOrDefault(user => user.RefreshTokenHash == refreshTokenHash));

    private static Option<User> ToOption(User? user) =>
        user is not null ? Option.Some(user) : Option.None<User>();
}

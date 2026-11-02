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

    public void Add(User user)
    {
        context.Users.Add(user);
    }
    public IEnumerable<User> GetAll() =>
        context.Users.ToList();

    public Option<User> GetById(Guid id)
    {
        var user = context.Users.FirstOrDefault(u => u.Id == id);
        return user is null ? Option.None<User>() : Option.Some(user);
    }

    public void Remove(User user) =>
        context.Users.Remove(user);
}
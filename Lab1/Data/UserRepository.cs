using Application.Domain;
using Application.Repositories;
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
}

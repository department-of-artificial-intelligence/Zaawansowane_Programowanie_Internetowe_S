using Application.Domain;

namespace Data;

public class UserRepository(AppDbContext context) : IUserRepository
{
    public void Add(User user)
    {
        context.Users.Add(user);
    }
}
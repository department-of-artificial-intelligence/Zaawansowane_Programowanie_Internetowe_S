using Application.Domain;
using Application.Utilities;

namespace Application.Repositories;

public interface IUserRepository
{
    void Add(User user);
    Option<User> GetById(Guid userId);
    Option<User> GetByName(string userName);
    Option<User> GetByRefreshToken(string refreshToken);
    void Remove(User user);
}

using Application.Domain;
using Application.Utilities;

namespace Application.Repositories;

public interface IUserRepository{
    void Add(User user);
    void Remove(User user);
    Option<User> GetById(Guid id);
    Option<User> GetByName(string userName);
    Option<User> GetByRefreshToken(string refreshToken);
}

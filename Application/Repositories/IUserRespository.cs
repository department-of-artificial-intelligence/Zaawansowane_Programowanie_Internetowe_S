using Application.Domain;
using Application.Utilities;

namespace Application.Repositories;

public interface IUserRepository
{
    void Add(User user);
    Option<User> GetByName(string userName);

    Option<User> GetById(Guid userId);
    void Remove(User user);
}